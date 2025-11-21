using OrbitSpace.WebApi.Constants;

namespace OrbitSpace.WebApi.Startup
{
    public static class CorsConfig
    {
        public static IServiceCollection AddAppCors(this IServiceCollection services, ConfigurationManager configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyConstants.PolicyName.AllowSpecificOrigins, policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }
    }
}
