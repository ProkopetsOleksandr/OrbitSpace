using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string hashedToken);
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task RevokeByTokenAsync(string hashedToken);
    Task RevokeAllByUserIdAsync(Guid userId);
    Task SaveChangesAsync();
}
