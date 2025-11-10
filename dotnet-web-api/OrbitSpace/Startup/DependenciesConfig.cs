using OrbitSpace.Application;
using OrbitSpace.Infrastructure;
using OrbitSpace.WebApi.Exceptions;
using OrbitSpace.WebApi.Identity;

namespace OrbitSpace.WebApi.Startup
{
    public static class DependenciesConfig
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddOpenApiServices();

            builder.Services.AddOptionServices(builder.Configuration);

            builder.Services.AddAuthenticationServices(builder.Configuration);

            builder.Services.AddMongoDbServices(builder.Configuration);

            builder.Services.AddApplication();
            
            builder.Services.AddInfrastructure();
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IApplicationUserProvider, ApplicationUserProvider>();
        }
    }
}
