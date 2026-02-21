using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Infrastructure.Persistence;
using OrbitSpace.Infrastructure.Persistence.Repositories;
using OrbitSpace.Infrastructure.Services;

namespace OrbitSpace.Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());

            services.AddSingleton<IEmailSenderService, EmailSenderService>()
                .AddSingleton<IEmailTemplateRenderService, EmailTemplateRenderService>()
                .AddSingleton<IPasswordHasherService, Argon2PasswordHasherService>()
                .AddSingleton<IJwtTokenService, JwtTokenService>()
                .AddSingleton<IFrontendUrlBuilder, FrontendUrlBuilder>();

            services.AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<ITodoItemRepository, TodoItemRepository>()
                .AddScoped<IGoalRepository, GoalRepository>()
                .AddScoped<IActivityRepository, ActivityRepository>()
                .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();

            return services;
        }
    }
}
