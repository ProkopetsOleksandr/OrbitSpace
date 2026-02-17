using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string username);
        Task CreateAsync(User user);
    }
}