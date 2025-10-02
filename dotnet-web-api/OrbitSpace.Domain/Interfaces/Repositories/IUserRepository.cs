using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task CreateAsync(User user);
    }
}
