using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task CreateAsync(User user);
}