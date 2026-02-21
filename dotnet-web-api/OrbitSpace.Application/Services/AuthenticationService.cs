using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Common.Utilities;
using OrbitSpace.Application.Dtos.Authentication;
using OrbitSpace.Application.Email.Templates;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Services;

public class AuthenticationService(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IPasswordHasherService passwordHasherService,
    IRefreshTokenRepository refreshTokenRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IEmailSenderService emailSenderService,
    IEmailTemplateRenderService emailTemplateRenderService,
    IFrontendUrlBuilder frontendUrlBuilder) : IAuthenticationService
{
    private const int StandardTokenLifetimeDays = 7;
    private const int RememberMeTokenLifetimeDays = 30;
    private static readonly TimeSpan EmailVerificationTokenLifetime = TimeSpan.FromHours(24);

    public async Task<OperationResult> RegisterAsync(RegisterRequestDto request)
    {
        if (await userRepository.FindByEmailAsync(request.Email) != null)
        {
            return OperationResultError.Validation("Email already exists");
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHasherService.HashPassword(request.Password),
            EmailVerified = false,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            IsActive = true
        };
        
        userRepository.Add(user);
        
        var (emailVerificationToken, hashedEmailVerificationToken) = SecureTokenGenerator.Generate();
        emailVerificationTokenRepository.Add(new EmailVerificationToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = hashedEmailVerificationToken,
            ExpiresAtUtc = now.Add(EmailVerificationTokenLifetime)
        });

        await unitOfWork.SaveChangesAsync();
        
        await SendEmailVerificationMessageAsync(user.FirstName, user.Email, emailVerificationToken);
        
        return OperationResult.Success();
    }

    public async Task<OperationResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.FindByEmailAsync(request.Email);
        if (user == null || passwordHasherService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return OperationResultError.Validation("Invalid username or password");
        }
        
        var accessToken = jwtTokenService.GenerateAccessToken(user.Id);
        var refreshTokenValue = jwtTokenService.GenerateRefreshToken();
        var hashedRefreshToken = jwtTokenService.HashToken(refreshTokenValue);
        
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

        refreshTokenRepository.Add(refreshToken);
        await unitOfWork.SaveChangesAsync();

        return new LoginResponseDto(accessToken, refreshTokenValue);
    }

    public async Task<OperationResult<RefreshResponseDto>> RefreshAsync(RefreshRequestDto request, Guid userId)
    {
        var hashedToken = jwtTokenService.HashToken(request.RefreshToken);
        var refreshToken = await refreshTokenRepository.FindByHashedTokenAsync(hashedToken);
        if (refreshToken?.IsActive != true || refreshToken.UserId != userId)
        {
            return OperationResultError.Unauthorized("Invalid or expired refresh token");
        }

        var newAccessToken = jwtTokenService.GenerateAccessToken(refreshToken.UserId);
        var newRefreshTokenValue = jwtTokenService.GenerateRefreshToken();
        var newHashedRefreshToken = jwtTokenService.HashToken(newRefreshTokenValue);
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
        
        refreshTokenRepository.Update(refreshToken);
        refreshTokenRepository.Add(newRefreshToken);
        await unitOfWork.SaveChangesAsync();

        return new RefreshResponseDto(newAccessToken, newRefreshTokenValue);
    }

    public async Task RevokeFamilyAsync(Guid familyId, TokenRevokedReason revokedReason)
    {
        var tokens = await refreshTokenRepository.GetByFamilyIdAsync(familyId);
        await RevokeRefreshTokensAsync(tokens, revokedReason);
    }

    public async Task RevokeTokenAsync(RevokeRequestDto request)
    {
        var hashedToken = jwtTokenService.HashToken(request.RefreshToken);
        var refreshToken =  await refreshTokenRepository.FindByHashedTokenAsync(hashedToken);
        if (refreshToken != null)
        {
            refreshToken.RevokedAtUtc = DateTime.UtcNow;
            refreshTokenRepository.Update(refreshToken);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RevokeAllForUserAsync(Guid userId, TokenRevokedReason tokenRevokedReason)
    {
        var tokens = await refreshTokenRepository.GetActiveByUserIdAsync(userId);
        await RevokeRefreshTokensAsync(tokens, tokenRevokedReason);
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

        refreshTokenRepository.Update(refreshTokens);
        await unitOfWork.SaveChangesAsync();
    }
    
    private async Task SendEmailVerificationMessageAsync(string firstName, string email, string token)
    {
        var body = emailTemplateRenderService.Render(new EmailVerificationTemplate
        {
            FirstName = firstName,
            ConfirmationUrl = frontendUrlBuilder.BuildEmailVerificationUrl(token)
        });

        await emailSenderService.SendAsync("Verify email", email, body);
    }
    
    private static int GetExpirationDays(bool rememberMe)
    {
        return rememberMe ? RememberMeTokenLifetimeDays : StandardTokenLifetimeDays;
    }
}