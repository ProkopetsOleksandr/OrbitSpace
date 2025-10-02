using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
