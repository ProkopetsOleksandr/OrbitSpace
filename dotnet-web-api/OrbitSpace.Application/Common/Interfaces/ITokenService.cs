using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(Guid userId);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}