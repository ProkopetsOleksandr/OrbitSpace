using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class GoalRepository(AppDbContext dbContext) : IGoalRepository
{
    public async Task<Goal?> FindByIdAsync(Guid id, Guid userId)
    {
        return await dbContext.Goals.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
    }
    
    public async Task<List<Goal>> GetAllAsync(Guid userId)
    {
        return await dbContext.Goals
            .Where(g => g.UserId == userId)
            .ToListAsync();
    }

    public void Add(Goal goal)
    {
        dbContext.Goals.Add(goal);
    }

    public void Update(Goal goal)
    {
        dbContext.Goals.Update(goal);
    }

    public async Task<int> DeleteAsync(Guid id, Guid userId)
    {
        return await dbContext.Goals
            .Where(g => g.Id == id && g.UserId == userId)
            .ExecuteDeleteAsync();
    }
}
