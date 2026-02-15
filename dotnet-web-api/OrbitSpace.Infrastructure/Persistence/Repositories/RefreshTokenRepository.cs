using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task CreateAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string hashedToken)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == hashedToken);
    }

    public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await context.RefreshTokens
            .Where(rt => rt.UserId == userId
                && rt.RevokedAtUtc == null
                && rt.UsedAtUtc == null
                && rt.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task RevokeByTokenAsync(string hashedToken)
    {
        var token = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == hashedToken);
        if (token != null)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task RevokeAllByUserIdAsync(Guid userId)
    {
        var tokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAtUtc == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
