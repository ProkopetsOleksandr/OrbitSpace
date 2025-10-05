using OrbitSpace.Domain.Interfaces.Repositories;
using OrbitSpace.Domain.Interfaces.Services;
using OrbitSpace.Infrastructure.Repositories;
using OrbitSpace.Infrastructure.Services;

namespace OrbitSpace.WebApi.Startup
{
    public static class ApplicationServicesConfig
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ITokenService, TokenService>();
        }
    }
}
