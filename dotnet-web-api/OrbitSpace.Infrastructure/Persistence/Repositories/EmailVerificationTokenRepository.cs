using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class EmailVerificationTokenRepository(AppDbContext context) : IEmailVerificationTokenRepository
{
    public void Add(EmailVerificationToken emailVerificationToken)
    {
        context.EmailVerificationTokens.Add(emailVerificationToken);
    }
}