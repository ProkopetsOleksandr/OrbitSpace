using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(Guid userId);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}