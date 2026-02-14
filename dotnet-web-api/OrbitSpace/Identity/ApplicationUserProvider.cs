using System.Security.Claims;

namespace OrbitSpace.WebApi.Identity;

public class ApplicationUserProvider : IApplicationUserProvider
{
    private readonly ClaimsPrincipal _claims;

    public ApplicationUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _claims = httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedAccessException();
    }

    private Guid TempUserId = Guid.CreateVersion7(new DateTimeOffset(2026, 2, 13, 14, 52, 0, TimeSpan.Zero));
    public Guid UserId => TempUserId; //Guid.Parse(GetValueFromClaim(ClaimTypes.NameIdentifier));

    public string UserEmail => GetValueFromClaim(ClaimTypes.Email);

    private string GetValueFromClaim(string type)
    {
        var value = _claims.FindFirst(type)?.Value;
        return string.IsNullOrWhiteSpace(value) ? throw new UnauthorizedAccessException() : value;
    }
}
