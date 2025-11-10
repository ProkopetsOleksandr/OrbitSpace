using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrbitSpace.Infrastructure.Settings;

namespace OrbitSpace.WebApi.Startup
{
    public static class AuthenticationConfig
    {
        public static void AddAuthenticationServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = jwtSettings.Issuer;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.Response.ContentType = "application/problem+json";
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                            var problem = new ProblemDetails
                            {
                                Type = "https://tools.ietf.org/html/rfc6750#section-3.1",
                                Title = "Unauthorized",
                                Status = 401,
                                Detail = "Invalid or missing token."
                            };

                            return context.Response.WriteAsJsonAsync(problem);
                        }
                    };
                }
            );
        }
    }
}