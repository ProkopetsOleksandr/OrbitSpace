using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class EmailVerificationTokenRepository(AppDbContext context) : IEmailVerificationTokenRepository
{
    public async Task CreateAsync(EmailVerificationToken emailVerificationToken)
    {
        context.EmailVerificationTokens.Add(emailVerificationToken);
        await context.SaveChangesAsync();
    }
}