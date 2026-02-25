using System.Security.Cryptography;

namespace OrbitSpace.Application.Common.Utilities;

public static class SecureTokenGenerator
{
    private const int TokenSizeInBytes = 32; // OWASP best practice for secure tokens
    
    public static (string rawToken, string hashedToken) Generate()
    {
        var rawBytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);
        var rawToken = Convert.ToBase64String(rawBytes);
        var hashedToken = Convert.ToBase64String(SHA256.HashData(rawBytes));
        return (rawToken, hashedToken);
    }

    public static string Hash(string rawToken)
    {
        try
        {
            var rawBytes = Convert.FromBase64String(rawToken);
            return Convert.ToBase64String(SHA256.HashData(rawBytes));
        }
        catch (FormatException)
        {
            throw new ArgumentException("Invalid token format.", nameof(rawToken));
        }
    }
}