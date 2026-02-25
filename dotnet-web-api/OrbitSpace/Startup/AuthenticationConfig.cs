using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrbitSpace.Infrastructure.Configuration;

namespace OrbitSpace.WebApi.Startup
{
    public static class AuthenticationConfig
    {
        public static void AddAuthenticationServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException("JWT options are missing.");

            var rsa = RSA.Create();
            rsa.ImportFromPem(jwtOptions.PublicKey);
            var rsaKey = new RsaSecurityKey(rsa);
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = rsaKey,
                        ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
                        
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            
                            context.Response.ContentType = "application/problem+json";
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.Headers["WWW-Authenticate"] = 
                                $"Bearer error=\"invalid_token\", error_description=\"Invalid or missing token.\"";
                            
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