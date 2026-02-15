using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos;
using OrbitSpace.Application.Dtos.Authentication;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Services;

public class AuthenticationService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordHasherService passwordHasherService,
    IRefreshTokenRepository refreshTokenRepository) : IAuthenticationService
{
    private const string InvalidUsernameOrPasswordMessage = "Invalid username or password.";

    public async Task<OperationResult> RegisterAsync(RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.FirstName)
            || string.IsNullOrWhiteSpace(request.LastName)
            || string.IsNullOrWhiteSpace(request.Password)
            || request.Password.Length < 4)
        {
            return OperationResultError.Validation("Invalid username or password");
        }

        if (await userRepository.GetByEmailAsync(request.Email) != null)
        {
            return OperationResultError.Validation("Email already exists");
        }

        var now = DateTime.UtcNow;
        await userRepository.CreateAsync(new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHasherService.HashPassword(request.Password),
            DateOfBirth = request.DateOfBirth,
            EmailVerified = false,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        });

        return OperationResult.Success();
    }

    public async Task<OperationResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return OperationResultError.Validation(InvalidUsernameOrPasswordMessage);
        }

        var passwordHasherResult = passwordHasherService.VerifyPassword(user.PasswordHash, request.Password);
        if (!passwordHasherResult)
        {
            return OperationResultError.Validation(InvalidUsernameOrPasswordMessage);
        }

        // Generate access token
        var accessToken = tokenService.GenerateAccessToken(user);

        // Generate refresh token
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var hashedRefreshToken = tokenService.HashToken(refreshTokenValue);

        // Determine expiration based on RememberMe
        var expirationDays = request.RememberMe ? 30 : 7;
        var now = DateTime.UtcNow;

        // Create refresh token entity
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = hashedRefreshToken,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(expirationDays),
            DeviceInfo = request.DeviceInfo
        };

        await refreshTokenRepository.CreateAsync(refreshToken);

        return new LoginResponseDto(
            accessToken,
            refreshTokenValue,
            new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                EmailVerified = user.EmailVerified
            }
        );
    }

    public async Task<OperationResult<RefreshResponseDto>> RefreshAsync(RefreshRequestDto request)
    {
        var hashedToken = tokenService.HashToken(request.RefreshToken);
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(hashedToken);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return OperationResultError.Unauthorized("Invalid or expired refresh token");
        }

        // Mark old token as used
        refreshToken.UsedAtUtc = DateTime.UtcNow;

        // Generate new tokens
        var newAccessToken = tokenService.GenerateAccessToken(refreshToken.User);
        var newRefreshTokenValue = tokenService.GenerateRefreshToken();
        var newHashedRefreshToken = tokenService.HashToken(newRefreshTokenValue);

        // Determine expiration strategy from old token (preserve RememberMe choice)
        var isRememberMe = refreshToken.ExpiresAtUtc > DateTime.UtcNow.AddDays(7);
        var expirationDays = isRememberMe ? 30 : 7;
        var now = DateTime.UtcNow;

        // Create new refresh token
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = refreshToken.UserId,
            Token = newHashedRefreshToken,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(expirationDays),
            DeviceInfo = refreshToken.DeviceInfo
        };

        // Set replacement tracking
        refreshToken.ReplacedByToken = newHashedRefreshToken;

        await refreshTokenRepository.CreateAsync(newRefreshToken);
        await refreshTokenRepository.SaveChangesAsync();

        return new RefreshResponseDto(newAccessToken, newRefreshTokenValue);
    }

    public async Task<OperationResult> RevokeAsync(RevokeRequestDto request)
    {
        var hashedToken = tokenService.HashToken(request.RefreshToken);
        await refreshTokenRepository.RevokeByTokenAsync(hashedToken);
        return OperationResult.Success();
    }

    public async Task<OperationResult> RevokeAllAsync(Guid userId)
    {
        await refreshTokenRepository.RevokeAllByUserIdAsync(userId);
        return OperationResult.Success();
    }
}