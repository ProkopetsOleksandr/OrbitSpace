using Microsoft.AspNetCore.Mvc;
using OrbitSpace.WebApi.Startup;

namespace OrbitSpace.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddDependencies();

            var app = builder.Build();
            app.UseExceptionHandler();
            
            app.UseSwagger();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode is >= 400 and < 600
                    && context.HttpContext.Response is { HasStarted: false, ContentLength: null })
                {
                    var problemDetailsService = context.HttpContext.RequestServices.GetService<IProblemDetailsService>();
                    if (problemDetailsService == null)
                    {
                        return;
                    }
                    
                    var problem = new ProblemDetailsContext
                    {
                        HttpContext = context.HttpContext,
                        ProblemDetails = new ProblemDetails
                        {
                            Status = context.HttpContext.Response.StatusCode,
                            Title = "An error occurred",
                        }
                    };
                    
                    await problemDetailsService.WriteAsync(problem);
                    if (context.HttpContext.Response.ContentLength > 0)
                    {
                        return;   
                    }
                    
                    context.HttpContext.Response.ContentType = "application/problem+json";
                    await context.HttpContext.Response.WriteAsJsonAsync(problem.ProblemDetails);
                }
            });

            app.UseAuthorization();

            app.MapControllers();
            
            app.Run();
        }
    }
}
