using OrbitSpace.Application.Interfaces.Repositories;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Application.Services;
using OrbitSpace.Infrastructure.Persistence.Repositories;
using OrbitSpace.Infrastructure.Services;

namespace OrbitSpace.WebApi.Startup
{
    public static class ApplicationServicesConfig
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITodoItemRepository, TodoItemRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasherService, BCryptPasswordHasherService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            
            services.AddScoped<ITodoItemService, TodoItemService>();
        }
    }
}
