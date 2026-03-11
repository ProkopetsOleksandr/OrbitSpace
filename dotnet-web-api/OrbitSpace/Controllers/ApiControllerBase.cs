using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Common;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.WebApi.Identity;

namespace OrbitSpace.WebApi.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class ApiControllerBase : ControllerBase
{
    protected ApplicationUser CurrentUser
    {
        get
        {
            if (field is null)
            {
                var applicationUserProvider = HttpContext.RequestServices.GetRequiredService<IApplicationUserProvider>();
                field = new ApplicationUser(applicationUserProvider);
            }

            return field;
        }
    }

    protected IActionResult GetErrorResponse(OperationResultError error) =>
        error.ErrorType switch
        {
            OperationResultErrorType.NotFound => NotFoundProblem(error.ErrorMessage),
            OperationResultErrorType.Unauthorized => UnauthorizedProblem(error.ErrorMessage),
            _ => BadRequestProblem(error.ErrorMessage, error.ErrorCode)
        };

    protected IActionResult NotFoundProblem(string? errorMessage)
    {
        return ProblemResult(
            statusCode: StatusCodes.Status404NotFound,
            title: "Not found",
            detail: errorMessage,
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            errorCode: ErrorCode.Common.NotFound);
    }

    // TODO: ErrorCode should go first
    protected IActionResult BadRequestProblem(string? errorMessage, string errorCode)
    {
        return ProblemResult(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad request",
            detail: errorMessage,
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            errorCode: errorCode);
    }

    protected IActionResult UnauthorizedProblem(string? errorMessage)
    {
        return ProblemResult(
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Unauthorized",
            detail: errorMessage,
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            errorCode: ErrorCode.Common.Unauthorized);
    }

    protected IActionResult ValidationProblem(ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return ValidationProblem(ModelState);
    }
    
    private IActionResult ProblemResult(int statusCode, string title, string? detail, string type, string errorCode)
    {
        var pd = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode,
            Type = type
        };
        pd.Extensions["errorCode"] = errorCode;
        
        return new ObjectResult(pd) { StatusCode = statusCode };
    }
}
