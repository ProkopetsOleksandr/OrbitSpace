using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Infrastructure.Settings;

namespace OrbitSpace.Infrastructure.Services;

public class JwtTokenService(IOptions<JwtSettings> jwtSettingsOptions) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettingsOptions.Value;

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id!),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var tokenHandler = new JsonWebTokenHandler();

        return tokenHandler.CreateToken(tokenDescriptor);
    }
}