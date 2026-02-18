using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.Authentication;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Services;

public class AuthenticationService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordHasherService passwordHasherService,
    IRefreshTokenRepository refreshTokenRepository) : IAuthenticationService
{
    private const int StandardTokenLifetimeDays = 7;
    private const int RememberMeTokenLifetimeDays = 30;

    public async Task<OperationResult> RegisterAsync(RegisterRequestDto request)
    {
        if (await userRepository.FindByEmailAsync(request.Email) != null)
        {
            return OperationResultError.Validation("Email already exists");
        }

        var now = DateTime.UtcNow;
        await userRepository.CreateAsync(new User
        {
            Id = Guid.CreateVersion7(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHasherService.HashPassword(request.Password),
            DateOfBirth = request.DateOfBirth,
            EmailVerified = false,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            IsActive = true
        });

        return OperationResult.Success();
    }

    public async Task<OperationResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.FindByEmailAsync(request.Email);
        if (user == null || passwordHasherService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return OperationResultError.Validation("Invalid username or password");
        }
        
        var accessToken = tokenService.GenerateAccessToken(user.Id);
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var hashedRefreshToken = tokenService.HashToken(refreshTokenValue);
        
        var expirationDays = GetExpirationDays(request.RememberMe);
        var now = DateTime.UtcNow;
        
        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = hashedRefreshToken,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(expirationDays),
            DeviceInfo = request.DeviceInfo
        };

        await refreshTokenRepository.CreateAsync(refreshToken);

        return new LoginResponseDto(accessToken, refreshTokenValue);
    }

    public async Task<OperationResult<RefreshResponseDto>> RefreshAsync(RefreshRequestDto request, Guid userId)
    {
        var hashedToken = tokenService.HashToken(request.RefreshToken);
        var refreshToken = await refreshTokenRepository.FindByHashedTokenAsync(hashedToken);
        if (refreshToken?.IsActive != true || refreshToken.UserId != userId)
        {
            return OperationResultError.Unauthorized("Invalid or expired refresh token");
        }

        var newAccessToken = tokenService.GenerateAccessToken(refreshToken.UserId);
        var newRefreshTokenValue = tokenService.GenerateRefreshToken();
        var newHashedRefreshToken = tokenService.HashToken(newRefreshTokenValue);
        var expirationDays = GetExpirationDays(refreshToken.RememberMe);
        var now = DateTime.UtcNow;
        
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            FamilyId = refreshToken.FamilyId,
            UserId = refreshToken.UserId,
            TokenHash = newHashedRefreshToken,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(expirationDays),
            DeviceInfo = refreshToken.DeviceInfo,
            AbsoluteExpiresAtUtc = refreshToken.AbsoluteExpiresAtUtc,
            RememberMe = refreshToken.RememberMe
        };
        
        refreshToken.UsedAtUtc = now;
        refreshToken.ReplacedByToken = newRefreshToken.Id;

        await refreshTokenRepository.RotateTokensAsync(refreshToken, newRefreshToken);

        return new RefreshResponseDto(newAccessToken, newRefreshTokenValue);
    }

    public async Task RevokeFamilyAsync(Guid familyId, TokenRevokedReason revokedReason)
    {
        var tokens = await refreshTokenRepository.GetByFamilyIdAsync(familyId);
        await RevokeRefreshTokensAsync(tokens, revokedReason);
    }

    public async Task RevokeTokenAsync(RevokeRequestDto request)
    {
        var hashedToken = tokenService.HashToken(request.RefreshToken);
        var refreshToken =  await refreshTokenRepository.FindByHashedTokenAsync(hashedToken);
        if (refreshToken != null)
        {
            refreshToken.RevokedAtUtc = DateTime.UtcNow;
            await refreshTokenRepository.UpdateAsync(refreshToken);
        }
    }

    public async Task RevokeAllForUserAsync(Guid userId, TokenRevokedReason tokenRevokedReason)
    {
        var tokens = await refreshTokenRepository.GetActiveByUserIdAsync(userId);
        await RevokeRefreshTokensAsync(tokens, tokenRevokedReason);
    }
    
    private int GetExpirationDays(bool rememberMe)
    {
        return rememberMe ? RememberMeTokenLifetimeDays : StandardTokenLifetimeDays;
    }

    private async Task RevokeRefreshTokensAsync(List<RefreshToken> refreshTokens, TokenRevokedReason revokedReason)
    {
        if (refreshTokens.Count == 0)
        {
            return;
        }
        
        var now = DateTime.UtcNow;
        foreach (var token in refreshTokens)
        {
            token.RevokedAtUtc = now;
            token.TokenRevokedReason = revokedReason;
        }

        await refreshTokenRepository.UpdateAsync(refreshTokens);
    }
}