using System.Text.RegularExpressions;
using MongoDB.Driver;
using OrbitSpace.Application.Interfaces.Repositories;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories
{
    public class UserRepository(IMongoDbContext dbContext) : IUserRepository
    {
        public async Task CreateAsync(User user)
        {
            await dbContext.Users.InsertOneAsync(user);
        }

        public async Task<User?> GetByEmailAsync(string username)
        {
            var regex = new Regex($"^{Regex.Escape(username)}$", RegexOptions.IgnoreCase);
            var filter = Builders<User>.Filter.Regex(u => u.Email, new MongoDB.Bson.BsonRegularExpression(regex));
            
            return await dbContext.Users.Find(filter).FirstOrDefaultAsync();
        }
    }
}
