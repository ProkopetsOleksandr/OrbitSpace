using OrbitSpace.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace OrbitSpace.Infrastructure.Services;

public class Argon2PasswordHasherService : IPasswordHasherService
{
    private const int MemorySize = 19456;      // KiB — recommended by OWASP for Argon2id
    private const int Iterations = 2;          // minimum for memory-hard mode
    private const int DegreeOfParallelism = 1;
    private const int HashSize = 32;           // bytes
    private const int SaltSize = 16;           // bytes
    
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = GetPasswordHash(password, salt);
        
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string hash)
    {
        var parts = hash.Split('.');
        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);
        
        var actualHash = GetPasswordHash(hash, salt);
        
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
    
    private static byte[] GetPasswordHash(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            MemorySize = MemorySize,
            Iterations = Iterations,
            DegreeOfParallelism = DegreeOfParallelism,
            Salt = salt
        };

        return argon2.GetBytes(HashSize);
    }
}
