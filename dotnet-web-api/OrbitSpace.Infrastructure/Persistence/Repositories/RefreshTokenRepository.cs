using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> FindByHashedTokenAsync(string hashedToken)
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(m => m.TokenHash == hashedToken);
    }
    
    public async Task<List<RefreshToken>> GetByFamilyIdAsync(Guid familyId)
    {
        return await context.RefreshTokens
            .Where(m => m.FamilyId == familyId)
            .ToListAsync();
    }

    public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await context.RefreshTokens
            .Where(m => m.UserId == userId
                        && !m.RevokedAtUtc.HasValue
                        && !m.UsedAtUtc.HasValue
                        && m.ExpiresAtUtc > now
                        && m.AbsoluteExpiresAtUtc > now)
            .OrderByDescending(m => m.CreatedAtUtc)
            .ToListAsync();
    }
    
    public void Add(RefreshToken refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
    }
    
    public void Update(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
    }
    
    public void Update(List<RefreshToken> refreshTokens)
    {
        context.RefreshTokens.UpdateRange(refreshTokens);
    }
}
