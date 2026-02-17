using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task CreateAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await dbContext.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string username)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == username.ToLower());
    }
}
