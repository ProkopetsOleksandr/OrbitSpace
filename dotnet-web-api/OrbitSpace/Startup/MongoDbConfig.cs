using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrbitSpace.Infrastructure.Persistence;
using OrbitSpace.Infrastructure.Settings;

namespace OrbitSpace.WebApi.Startup
{
    public static class MongoDbConfig
    {
        public static void AddMongoDbServices(this IServiceCollection services, ConfigurationManager configuration)
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
}
