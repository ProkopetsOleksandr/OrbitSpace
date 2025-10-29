using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Models.Responses;

namespace OrbitSpace.WebApi.Controllers;

[ApiController]
public class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleFailure(OperationResultError error)
    {
        return error.ErrorType switch
        {
            OperationResultErrorType.NotFound => NotFound(error.ErrorMessage),
            OperationResultErrorType.Validation => BadRequest(error.ErrorMessage),
            OperationResultErrorType.Unauthorized => Unauthorized(error.ErrorMessage),
            OperationResultErrorType.Internal => StatusCode(StatusCodes.Status500InternalServerError,
                error.ErrorMessage),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}