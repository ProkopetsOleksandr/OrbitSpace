using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
    }
}