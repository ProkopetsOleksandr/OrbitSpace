using MongoDB.Driver;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories
{
    public class ActivityRepository(IMongoDbContext dbContext) : IActivityRepository
    {
        public async Task<List<Activity>> GetAllAsync(string userId)
        {
            var filter = Builders<Activity>.Filter.Eq(x => x.UserId, userId);

            return await dbContext.Activities.Find(filter).ToListAsync();
        }

        public async Task<Activity?> GetByIdAsync(string id)
        {
            var filter = Builders<Activity>.Filter.Eq(x => x.Id, id);

            return await dbContext.Activities.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Activity> CreateAsync(Activity activity)
        {
            await dbContext.Activities.InsertOneAsync(activity);

            return activity;
        }

        public async Task<bool> UpdateAsync(Activity activity)
        {
            var filter = Builders<Activity>.Filter.Eq(x => x.Id, activity.Id);
            var updateResult = await dbContext.Activities.ReplaceOneAsync(filter, activity);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            var filter = Builders<Activity>.Filter.And(
                Builders<Activity>.Filter.Eq(x => x.Id, id),
                Builders<Activity>.Filter.Eq(x => x.UserId, userId));

            var deleteResult = await dbContext.Activities.DeleteOneAsync(filter);

            return deleteResult.DeletedCount > 0;
        }
    }
}
