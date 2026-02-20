using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using OrbitSpace.Application.Services;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Application.Validators.Authentication;

namespace OrbitSpace.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddMapsterServices();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITodoItemService, TodoItemService>();
            services.AddScoped<IGoalService, GoalService>();
            services.AddScoped<IActivityService, ActivityService>();

            return services;
        }
        
        private IServiceCollection AddMapsterServices()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(DependencyInjection).Assembly);
            config.Compile();

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}