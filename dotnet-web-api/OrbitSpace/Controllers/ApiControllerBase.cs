using System.Net;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Models.Responses;
using OrbitSpace.WebApi.Identity;
using OrbitSpace.WebApi.Models.Responses.Base;

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

    protected IActionResult ApiErrorResponse(OperationResultError error)
    {
        if (error.ErrorType == OperationResultErrorType.NotFound)
        {
            return new NotFoundObjectResult(new ApiErrorResponse(new ApiError((int)HttpStatusCode.NotFound, error.ErrorMessage)));
        }

        if (error.ErrorType == OperationResultErrorType.Unauthorized)
        {
            return new UnauthorizedObjectResult(new ApiErrorResponse(new ApiError((int)HttpStatusCode.NotFound, error.ErrorMessage)));
        }

        return new BadRequestObjectResult(new ApiErrorResponse(new ApiError((int)HttpStatusCode.BadRequest, error.ErrorMessage)));
    }
}