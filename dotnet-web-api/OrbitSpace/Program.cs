using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OrbitSpace.Domain.Interfaces.Repositories;
using OrbitSpace.Domain.Interfaces.Services;
using OrbitSpace.Infrastructure.Repositories;
using OrbitSpace.Infrastructure.Services;
using OrbitSpace.Infrastructure.Settings;

namespace OrbitSpace.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            RegisterServices(builder);

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                            .GetBytes(builder.Configuration["JwtSettings:Key"]!)),
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        ValidateLifetime = true
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        public static void RegisterServices(WebApplicationBuilder builder)
        {
            var jwtSettingsSection = builder.Configuration.GetSection(JwtSettings.SectionName);
            builder.Services.Configure<JwtSettings>(jwtSettingsSection);

            var mongoDbSettingsSection = builder.Configuration.GetSection(MongoDbSettings.SectionName);
            var connectionString = mongoDbSettingsSection.GetValue<string>(nameof(MongoDbSettings.ConnectionString));
            var databaseName = mongoDbSettingsSection.GetValue<string>(nameof(MongoDbSettings.DatabaseName));

            builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                return new MongoClient(connectionString);
            });

            builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
            {
                var client = serviceProvider.GetRequiredService<IMongoClient>();
                return client.GetDatabase(databaseName);
            });

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
        }
    }
}
