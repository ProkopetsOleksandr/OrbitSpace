using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task CreateAsync(User user)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(User user)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        return await dbContext.Users.FindAsync(id);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
