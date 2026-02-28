using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> FindByIdAsync(Guid id)
    {
        return await dbContext.Users.FindAsync(id);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await FindByIdAsync(id) ?? throw new InvalidOperationException($"User with id '{id}' was not found.");
    }
    
    public void Add(User user)
    {
        dbContext.Users.Add(user);
    }
    
    public void Update(User user)
    {
        dbContext.Users.Update(user);
    }
}
