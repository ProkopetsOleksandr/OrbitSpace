using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task<User?> FindByIdAsync(Guid id);
        Task<User?> FindByEmailAsync(string email);
    }
}