using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindByHashedTokenAsync(string hashedToken);
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task<List<RefreshToken>> GetByFamilyIdAsync(Guid familyId);
    void Add(RefreshToken refreshToken);
    void Update(RefreshToken refreshToken);
    void Update(List<RefreshToken> refreshTokens);
}
