using System.Text.Json.Serialization;
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

            services.AddControllers();
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddOpenApiServices();

            services.AddHttpContextAccessor();
            services.AddScoped<IApplicationUserProvider, ApplicationUserProvider>();

            return services;
        }
    }
}
