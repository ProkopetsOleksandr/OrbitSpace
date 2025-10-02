using MongoDB.Driver;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Domain.Interfaces.Repositories;

namespace OrbitSpace.Infrastructure.Repositories
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

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _usersCollection.Find(u => u.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();
        }
    }
}
