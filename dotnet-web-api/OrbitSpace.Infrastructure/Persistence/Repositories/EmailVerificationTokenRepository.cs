using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class EmailVerificationTokenRepository(AppDbContext context) : IEmailVerificationTokenRepository
{
    public void Add(EmailVerificationToken emailVerificationToken)
    {
        context.EmailVerificationTokens.Add(emailVerificationToken);
    }

    public async Task<EmailVerificationToken?> FindEmailByTokenHashAsync(string hashedToken)
    {
        return await context.EmailVerificationTokens.Where(m => m.TokenHash == hashedToken).FirstOrDefaultAsync();
    }
}