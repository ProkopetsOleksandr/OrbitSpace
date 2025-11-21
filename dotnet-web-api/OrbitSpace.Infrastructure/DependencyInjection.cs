using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Infrastructure.Persistence;
using OrbitSpace.Infrastructure.Persistence.Repositories;
using OrbitSpace.Infrastructure.Services;
using OrbitSpace.Infrastructure.Settings;

namespace OrbitSpace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMongoDbServices();

        services.AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ITodoItemRepository, TodoItemRepository>()
            .AddScoped<ITokenService, JwtTokenService>()
            .AddScoped<IPasswordHasherService, BCryptPasswordHasherService>();

        return services;
    }

    private static void AddMongoDbServices(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });

        services.AddScoped<IMongoDbContext, MongoDbContext>();

        MongoMappingsConfig.RegisterAll();
    }
}