using OrbitSpace.WebApi.Startup;

namespace OrbitSpace.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddDependencies();

            var app = builder.Build();
            app.UseExceptionHandler();
            
            app.UseSwagger();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();
            
            app.Run();
        }
    }
}
