using OrbitSpace.Application.Common.Configuration;
using OrbitSpace.Infrastructure.Configuration;

namespace OrbitSpace.WebApi.Startup
{
    public static class OptionsConfig
    {
        public static void AddOptionServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName))
                .Validate(settings =>
                    !string.IsNullOrWhiteSpace(settings.Key)
                    && !string.IsNullOrWhiteSpace(settings.Issuer)
                    && !string.IsNullOrWhiteSpace(settings.Audience),
                "JWT token options are missing.")
                .ValidateOnStart();
            
            services.AddOptions<FrontendOptions>()
                .Bind(configuration.GetSection(FrontendOptions.SectionName))
                .Validate(options =>
                    !string.IsNullOrWhiteSpace(options.BaseUrl)
                    && !string.IsNullOrWhiteSpace(options.EmailVerificationUrlTemplate),
                    "Frontend options are missing.")
                .ValidateOnStart();

            services.AddOptions<SmtpOptions>()
                .Bind(configuration.GetSection(SmtpOptions.SectionName))
                .Validate(options =>
                    !string.IsNullOrWhiteSpace(options.Host)
                    && options.Port > 0
                    && options.Port <= 65535
                    && !string.IsNullOrWhiteSpace(options.From),
                    "SMTP options are missing")
                .ValidateOnStart();
        }
    }
}
