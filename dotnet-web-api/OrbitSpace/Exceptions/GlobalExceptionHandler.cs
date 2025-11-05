using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OrbitSpace.WebApi.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception");

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error",
            Detail = !string.IsNullOrEmpty(exception.Message) ? exception.Message : "An unexpected error occurred.",
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };
        
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        };

        if (await problemDetailsService.TryWriteAsync(context))
        {
            return true;
        }

        // Fallback (если вдруг не сработало)
        httpContext.Response.StatusCode = 500;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}