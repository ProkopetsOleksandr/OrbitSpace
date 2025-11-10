using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.WebApi.Identity;

namespace OrbitSpace.WebApi.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class ApiControllerBase : ControllerBase
{
    private ApplicationUser? _currentUser;
    protected ApplicationUser CurrentUser
    {
        get
        {
            if (_currentUser is null)
            {
                var applicationUserProvider = HttpContext.RequestServices.GetRequiredService<IApplicationUserProvider>();
                _currentUser = new ApplicationUser(applicationUserProvider);
            }

            return _currentUser;
        }
    }

    protected IActionResult HandleError(OperationResultError error)
    {
        var (statusCode, title) = MapErrorStatusCodeAndTitle(error.ErrorType);
        
        return Problem(
            detail: error.ErrorMessage,
            instance:HttpContext.Request.Path,
            statusCode: statusCode,
            title: title,
            type: error.ErrorType.ToString());
    }

    private static (int, string) MapErrorStatusCodeAndTitle(OperationResultErrorType errorType)
    {
        switch (errorType)
        {
            case OperationResultErrorType.NotFound: return (StatusCodes.Status404NotFound, "Resource not found");
            case OperationResultErrorType.Unauthorized: return (StatusCodes.Status401Unauthorized, "Unauthorized");
            case OperationResultErrorType.Validation:
            default: return (StatusCodes.Status400BadRequest, "Bad request");
        };
    }
}