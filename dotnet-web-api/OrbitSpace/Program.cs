using OrbitSpace.WebApi.Startup;
using OrbitSpace.Application;
using OrbitSpace.Infrastructure;

namespace OrbitSpace.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddPresentation(builder.Configuration)
                .AddApplication()
                .AddInfrastructure();

            var app = builder.Build();
            app.UseHttpsRedirection();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }
            
            app.UseStatusCodePages();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            
            app.UseOpenApi();
            
            app.Run();
        }
    }
}
