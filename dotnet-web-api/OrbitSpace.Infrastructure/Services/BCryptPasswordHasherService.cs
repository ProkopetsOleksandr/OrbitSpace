using OrbitSpace.Application.Interfaces.Services;

namespace OrbitSpace.Infrastructure.Services
{
    public class BCryptPasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }   
}