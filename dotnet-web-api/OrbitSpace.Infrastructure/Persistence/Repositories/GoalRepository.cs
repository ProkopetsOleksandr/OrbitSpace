using MongoDB.Driver;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories
{
    public class GoalRepository(IMongoDbContext dbContext) : IGoalRepository
    {
        public async Task<List<Goal>> GetAllAsync(string userId)
        {
            var filter = Builders<Goal>.Filter.Eq(x => x.UserId, userId);

            return await dbContext.Goals.Find(filter).ToListAsync();
        }

        public async Task<Goal?> GetByIdAsync(string id)
        {
            var filter = Builders<Goal>.Filter.Eq(x => x.Id, id);
        
            return await dbContext.Goals.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Goal> CreateAsync(Goal goal)
        {
            await dbContext.Goals.InsertOneAsync(goal);
        
            return goal;
        }

        public async Task<bool> UpdateAsync(Goal goal)
        {
            var filter = Builders<Goal>.Filter.Eq(x => x.Id, goal.Id);
            var updateResult = await dbContext.Goals.ReplaceOneAsync(filter, goal);
        
            return updateResult.IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            var filter = Builders<Goal>.Filter.And(
                Builders<Goal>.Filter.Eq(x => x.Id, id),
                Builders<Goal>.Filter.Eq(x => x.UserId, userId));
        
            var deleteResult = await dbContext.Goals.DeleteOneAsync(filter);

            return deleteResult.DeletedCount > 0;
        }
    }
}
