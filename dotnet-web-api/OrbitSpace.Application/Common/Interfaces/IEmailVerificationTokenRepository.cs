using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces;

public interface IEmailVerificationTokenRepository
{
    Task CreateAsync(EmailVerificationToken emailVerificationToken);
}