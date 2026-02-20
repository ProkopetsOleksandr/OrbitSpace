using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces;

public interface IEmailVerificationTokenRepository
{
    void Add(EmailVerificationToken emailVerificationToken);
}