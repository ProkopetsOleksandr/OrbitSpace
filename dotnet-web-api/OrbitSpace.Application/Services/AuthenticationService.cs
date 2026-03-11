using OrbitSpace.Application.Common;
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
    private const int StandardTokenLifetimeDays = 1;
    private const int StandardTokenAbsoluteLifetimeDays = 7;
    private const int LongLivedTokenLifetimeDays = 30;
    private const int LongLivedTokenAbsoluteLifetimeDays = 90;
    private static readonly TimeSpan EmailVerificationTokenLifetime = TimeSpan.FromHours(24);

    public async Task<OperationResult> RegisterAsync(RegisterRequestDto request)
    {
        if (await userRepository.FindByEmailAsync(request.Email) != null)
        {
            return OperationResultError.Validation("Email already exists", ErrorCode.Auth.EmailAlreadyExists);
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
        if (user == null)
        {
            return OperationResultError.Validation("Invalid username or password", ErrorCode.Auth.InvalidCredentials);
        }

        if (!user.EmailVerified)
        {
            return OperationResultError.Validation("Email not verified", ErrorCode.Auth.EmailNotVerified);
        }

        if (!passwordHasherService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return OperationResultError.Validation("Invalid username or password", ErrorCode.Auth.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return OperationResultError.Validation("Account disabled", ErrorCode.Auth.AccountDisabled);
        }
        
        var accessToken = jwtTokenService.GenerateAccessToken(user.Id);
        var (refreshTokenValue, hashedRefreshTokenValue) = SecureTokenGenerator.Generate();
        
        var now = DateTime.UtcNow;
        
        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            FamilyId = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = hashedRefreshTokenValue,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(request.RememberMe ? LongLivedTokenLifetimeDays : StandardTokenLifetimeDays),
            DeviceInfo = request.DeviceInfo,
            IsLongLived = request.RememberMe,
            AbsoluteExpiresAtUtc = now.AddDays(request.RememberMe ? LongLivedTokenAbsoluteLifetimeDays : StandardTokenAbsoluteLifetimeDays),
        };

        refreshTokenRepository.Add(refreshToken);
        await unitOfWork.SaveChangesAsync();

        return new LoginResponseDto(accessToken, refreshTokenValue);
    }

    public async Task<OperationResult<RefreshResponseDto>> RefreshAsync(RefreshRequestDto request, Guid userId)
    {
        var hashedToken = SecureTokenGenerator.Hash(request.RefreshToken);
        var refreshToken = await refreshTokenRepository.FindByHashedTokenAsync(hashedToken);
        if (refreshToken?.IsActive != true || refreshToken.UserId != userId)
        {
            if (refreshToken != null && (refreshToken.IsRevoked || refreshToken.IsUsed))
            {
                // Reuse detection.
                await RevokeFamilyAsync(refreshToken.FamilyId, TokenRevokedReason.TokenReuse);
            }
            
            return OperationResultError.Unauthorized("Invalid or expired refresh token", ErrorCode.Auth.InvalidRefreshToken);
        }

        var newAccessToken = jwtTokenService.GenerateAccessToken(refreshToken.UserId);
        var (newRefreshTokenValue, newHashedRefreshToken) = SecureTokenGenerator.Generate();
        var now = DateTime.UtcNow;
        
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            FamilyId = refreshToken.FamilyId,
            UserId = refreshToken.UserId,
            TokenHash = newHashedRefreshToken,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(refreshToken.IsLongLived ? LongLivedTokenLifetimeDays : StandardTokenLifetimeDays),
            DeviceInfo = refreshToken.DeviceInfo,
            AbsoluteExpiresAtUtc = refreshToken.AbsoluteExpiresAtUtc,
            IsLongLived = refreshToken.IsLongLived
        };
        
        refreshToken.UsedAtUtc = now;
        refreshToken.ReplacedByToken = newRefreshToken.Id;
        
        refreshTokenRepository.Update(refreshToken);
        refreshTokenRepository.Add(newRefreshToken);
        await unitOfWork.SaveChangesAsync();

        return new RefreshResponseDto(newAccessToken, newRefreshTokenValue);
    }

    public async Task LogoutAsync(LogoutRequestDto request)
    {
        var hashedToken = SecureTokenGenerator.Hash(request.RefreshToken);
        var refreshToken =  await refreshTokenRepository.FindByHashedTokenAsync(hashedToken);
        if (refreshToken != null)
        {
            await RevokeFamilyAsync(refreshToken.FamilyId, TokenRevokedReason.UserLogout);
        }
    }

    public async Task<OperationResult> VerifyEmailAsync(string token)
    {
        var hashedToken = SecureTokenGenerator.Hash(token);
        var emailVerificationToken = await emailVerificationTokenRepository.FindEmailByTokenHashAsync(hashedToken);

        if (emailVerificationToken == null || emailVerificationToken.IsExpired) {
            return OperationResultError.Validation("Verification link has expired. Please request a new one.", ErrorCode.Auth.VerificationLinkExpired);
        }

        if (emailVerificationToken.IsUsed)
        {
            return OperationResultError.Validation("Email already verified", ErrorCode.Auth.EmailAlreadyVerified);
        }

        emailVerificationToken.IsUsed = true;
        
        var user = await userRepository.GetByIdAsync(emailVerificationToken.UserId);
        user.EmailVerified = true;

        await unitOfWork.SaveChangesAsync();

        return OperationResult.Success();
    }

    public async Task ResendVerificationEmailAsync(string email)
    {
        var user = await userRepository.FindByEmailAsync(email);
        if (user == null || user.EmailVerified)
        {
            return;
        }

        var (emailVerificationToken, hashedEmailVerificationToken) = SecureTokenGenerator.Generate();
        emailVerificationTokenRepository.Add(new EmailVerificationToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = hashedEmailVerificationToken,
            ExpiresAtUtc = DateTime.UtcNow.Add(EmailVerificationTokenLifetime)
        });

        await unitOfWork.SaveChangesAsync();

        await SendEmailVerificationMessageAsync(user.FirstName, user.Email, emailVerificationToken);
    }
    
    private async Task RevokeFamilyAsync(Guid familyId, TokenRevokedReason revokedReason)
    {
        var tokens = await refreshTokenRepository.GetByFamilyIdAsync(familyId);
        await RevokeRefreshTokensAsync(tokens, revokedReason);
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
}