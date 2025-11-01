namespace OrbitSpace.WebApi.Startup
{
    public static class DependenciesConfig
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            builder.Services.AddSwaggerServices();

            builder.Services.AddOptionServices(builder.Configuration);

            builder.Services.AddAuthenticationServices(builder.Configuration);

            builder.Services.AddMongoDbServices(builder.Configuration);

            builder.Services.AddApplicationServices();
        }
    }
}
