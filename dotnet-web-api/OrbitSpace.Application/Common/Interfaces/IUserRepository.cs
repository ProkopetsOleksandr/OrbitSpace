using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindByIdAsync(Guid id);
        Task<User?> FindByEmailAsync(string email);
        Task<User> GetByIdAsync(Guid id);
        void Add(User user);
        void Update(User user);
    }
}