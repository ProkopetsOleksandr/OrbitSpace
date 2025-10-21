using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string username);
    Task CreateAsync(User user);
}