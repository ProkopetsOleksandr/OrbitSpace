using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using OrbitSpace.Application.Services;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMapsterServices();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITodoItemService, TodoItemService>();
        services.AddScoped<IGoalService, GoalService>();
        services.AddScoped<IActivityService, ActivityService>();

        return services;
    }

    private static IServiceCollection AddMapsterServices(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        config.Compile();

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}