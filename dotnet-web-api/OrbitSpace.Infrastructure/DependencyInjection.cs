using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Infrastructure.Persistence;
using OrbitSpace.Infrastructure.Persistence.Repositories;
using OrbitSpace.Infrastructure.Services;

namespace OrbitSpace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<ITokenService, JwtTokenService>()
            .AddScoped<IPasswordHasherService, BCryptPasswordHasherService>();

        services.AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ITodoItemRepository, TodoItemRepository>()
            .AddScoped<IGoalRepository, GoalRepository>()
            .AddScoped<IActivityRepository, ActivityRepository>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}
