using System.Security.Claims;

namespace OrbitSpace.WebApi.Identity;

public class ApplicationUserProvider : IApplicationUserProvider
{
    private readonly ClaimsPrincipal _claims;

    public ApplicationUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _claims = httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedAccessException();
    }

    public string UserId => GetValueFromClaim(ClaimTypes.NameIdentifier);
    public string UserEmail => GetValueFromClaim(ClaimTypes.Email);

    private string GetValueFromClaim(string type)
    {
        var value = _claims.FindFirst(type)?.Value;
        return string.IsNullOrWhiteSpace(value) ? throw new UnauthorizedAccessException() : value;
    }
}