using System.Text.Json.Serialization;
using Microsoft.AspNetCore.RateLimiting;
using OrbitSpace.WebApi.Constants;
using OrbitSpace.WebApi.Exceptions;
using OrbitSpace.WebApi.Identity;

namespace OrbitSpace.WebApi.Startup
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAppCors(configuration);

            services.AddOptionServices(configuration);

            services.AddAuthenticationServices(configuration);
            
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(PolicyConstants.RateLimiting.Auth, limiterOptions =>
                {
                    limiterOptions.PermitLimit = 5;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueLimit = 0;
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = (context) =>
                {
                    var problemDetails = context.ProblemDetails;
                    var request = context.HttpContext.Request;
                    problemDetails.Instance ??= $"{request.Method} {request.Path}";
                };
            });

            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddOpenApiServices();

            services.AddHttpContextAccessor();
            services.AddScoped<IApplicationUserProvider, ApplicationUserProvider>();

            return services;
        }
    }
}
