using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Models.Responses;
using OrbitSpace.WebApi.Identity;
using OrbitSpace.WebApi.Models;

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
    
    protected ApiResponse<IEnumerable<T>> GetApiResponse<T>(T data)
    {
        if (data == null)
        {
            return ApiResponse<IEnumerable<T>>.Success(Array.Empty<T>());
        }

        var enumerable = data as IEnumerable<T>;
        if (enumerable != null)
        {
            return ApiResponse<IEnumerable<T>>.Success(enumerable);
        }

        return ApiResponse<IEnumerable<T>>.Success(new[] { data });
    }
    
     protected ObjectResult HandleFailure(OperationResultError error)
     {
         var data = ApiResponse.Failure(error.ErrorMessage);
         
         return error.ErrorType switch
         {
             OperationResultErrorType.NotFound => NotFound(data),
             OperationResultErrorType.Unauthorized => Unauthorized(data),
             _ => BadRequest(data)
         };
     }
}