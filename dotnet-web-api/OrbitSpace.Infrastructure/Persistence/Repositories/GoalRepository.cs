using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class GoalRepository(AppDbContext dbContext) : IGoalRepository
{
    public async Task<List<Goal>> GetAllAsync(Guid userId)
    {
        return await dbContext.Goals
            .Where(g => g.UserId == userId)
            .ToListAsync();
    }

    public async Task<Goal?> GetByIdAsync(Guid id)
    {
        return await dbContext.Goals.FindAsync(id);
    }

    public async Task<Goal> CreateAsync(Goal goal)
    {
        dbContext.Goals.Add(goal);
        await dbContext.SaveChangesAsync();
        return goal;
    }

    public async Task<bool> UpdateAsync(Goal goal)
    {
        dbContext.Goals.Update(goal);
        var affected = await dbContext.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var affected = await dbContext.Goals
            .Where(g => g.Id == id && g.UserId == userId)
            .ExecuteDeleteAsync();
        return affected > 0;
    }
}
