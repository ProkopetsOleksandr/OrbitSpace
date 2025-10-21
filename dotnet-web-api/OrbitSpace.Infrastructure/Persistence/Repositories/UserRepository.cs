using MongoDB.Driver;
using OrbitSpace.Application.Interfaces.Repositories;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepository(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<User>("users");
        }

        public async Task CreateAsync(User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task<User?> GetByEmailAsync(string username)
        {
            return await _usersCollection.Find(u => u.Email.ToLower() == username.ToLower()).FirstOrDefaultAsync();
        }
    }
}
