using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Infrastructure.Configuration;

namespace OrbitSpace.Infrastructure.Services;

public class JwtTokenService(IOptions<JwtOptions> jwtSettingsOptions) : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions = jwtSettingsOptions.Value;

    public string GenerateAccessToken(Guid userId)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(_jwtOptions.PrivateKey);
        var rsaKey = new RsaSecurityKey(rsa);
        
        var now = DateTime.UtcNow;
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        };
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            Expires = now.AddMinutes(15),
            SigningCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience
        };

        var tokenHandler = new JsonWebTokenHandler();

        return tokenHandler.CreateToken(tokenDescriptor);
    }
}