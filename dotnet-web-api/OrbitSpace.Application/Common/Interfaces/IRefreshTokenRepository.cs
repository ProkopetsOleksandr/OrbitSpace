using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task UpdateAsync(List<RefreshToken> refreshTokens);
    Task<RefreshToken?> FindByHashedTokenAsync(string hashedToken);
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task<List<RefreshToken>> GetByFamilyIdAsync(Guid familyId);
    Task RotateTokensAsync(RefreshToken oldToken, RefreshToken newToken);
}
