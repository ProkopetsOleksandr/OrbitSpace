using Microsoft.Extensions.DependencyInjection;
using OrbitSpace.Application.Services;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITodoItemService, TodoItemService>();

        return services;
    }
}