using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class ActivityRepository(AppDbContext dbContext) : IActivityRepository
{
    public async Task<List<Activity>> GetAllAsync(Guid userId)
    {
        return await dbContext.Activities
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<Activity?> GetByIdAsync(Guid id)
    {
        return await dbContext.Activities.FindAsync(id);
    }

    public async Task<Activity> CreateAsync(Activity activity)
    {
        dbContext.Activities.Add(activity);
        await dbContext.SaveChangesAsync();
        return activity;
    }

    public async Task<bool> UpdateAsync(Activity activity)
    {
        dbContext.Activities.Update(activity);
        var affected = await dbContext.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var affected = await dbContext.Activities
            .Where(a => a.Id == id && a.UserId == userId)
            .ExecuteDeleteAsync();
        return affected > 0;
    }
}
