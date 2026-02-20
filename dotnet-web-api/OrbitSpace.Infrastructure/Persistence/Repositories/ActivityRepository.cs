using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class ActivityRepository(AppDbContext dbContext) : IActivityRepository
{
    public async Task<Activity?> FindByIdAsync(Guid id, Guid userId)
    {
        return await dbContext.Activities.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
    }
    
    public async Task<List<Activity>> GetAllAsync(Guid userId)
    {
        return await dbContext.Activities
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }
    
    public void Add(Activity activity)
    {
        dbContext.Activities.Add(activity);
    }

    public void Update(Activity activity)
    {
        dbContext.Activities.Update(activity);
    }

    public async Task<int> DeleteAsync(Guid id, Guid userId)
    {
        return await dbContext.Activities
            .Where(a => a.Id == id && a.UserId == userId)
            .ExecuteDeleteAsync();
    }
}
