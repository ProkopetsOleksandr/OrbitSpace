using MongoDB.Driver;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories
{
    public class GoalResository(IMongoDbContext dbContext) : IGoalRepository
    {
        public async Task<List<Goal>> GetAllAsync(string userId)
        {
            var filter = Builders<Goal>.Filter.Eq(x => x.UserId, userId);

            return await dbContext.Goals.Find(filter).ToListAsync();
        }
    }
}
