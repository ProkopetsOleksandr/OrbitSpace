using System.Net;
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
        return error.ErrorType switch
        {
            OperationResultErrorType.NotFound => NotFoundProblem(error.ErrorMessage),
            _ => BadRequestProblem(error.ErrorMessage)
        };
    }

    protected IActionResult NotFoundProblem(string? errorMessage)
    {
        return Problem(
            title: "Not found",
            detail: errorMessage,
            statusCode: StatusCodes.Status404NotFound,
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.5");
    }
    
    protected IActionResult BadRequestProblem(string? errorMessage)
    {
        return Problem(
            title: "Bad request",
            detail: errorMessage,
            statusCode: StatusCodes.Status400BadRequest,
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.1");
    }
}