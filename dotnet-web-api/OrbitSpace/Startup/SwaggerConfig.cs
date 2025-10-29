namespace OrbitSpace.WebApi.Startup
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddOpenApi();
            services.AddSwaggerGen();
        }

        public static void UseSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Orbit Space API V1");
                    options.RoutePrefix = string.Empty;
                });
            }
        }
    }
}
