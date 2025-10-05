﻿using OrbitSpace.Infrastructure.Settings;

namespace OrbitSpace.WebApi.Startup
{
    public static class OptionsConfig
    {
        public static void AddOptionServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddOptions<JwtSettings>()
                .Bind(configuration.GetSection(JwtSettings.SectionName))
                .Validate(settings =>
                {
                    return !string.IsNullOrWhiteSpace(settings.Key)
                        && !string.IsNullOrWhiteSpace(settings.Issuer)
                        && !string.IsNullOrWhiteSpace(settings.Audience);
                },
                "Jwt token settings are missing.");

            services.AddOptions<MongoDbSettings>()
                .Bind(configuration.GetSection(MongoDbSettings.SectionName))
                .Validate(settings =>
                {
                    return !string.IsNullOrWhiteSpace(settings.ConnectionString) && !string.IsNullOrWhiteSpace(settings.DatabaseName);
                },
                "ConnectionString or DatabaseName is missing in MongoDbSettings.");
        }
    }
}
