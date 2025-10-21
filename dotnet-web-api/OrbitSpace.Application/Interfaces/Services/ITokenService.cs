using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}