using Microsoft.AspNetCore.Mvc;
using OrbitSpace.WebApi.Identity;

namespace OrbitSpace.WebApi.Controllers;

[ApiController]
public class ApiControllerBase : ControllerBase
{
    protected readonly ApplicationUser ApplicationUser;

    public ApiControllerBase()
    {
        var applicationUserProvider = HttpContext.RequestServices.GetRequiredService<IApplicationUserProvider>();
        ApplicationUser = new ApplicationUser(applicationUserProvider);
    }
}