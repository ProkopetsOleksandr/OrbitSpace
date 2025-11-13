using OrbitSpace.WebApi.OpenApi;
using Scalar.AspNetCore;

namespace OrbitSpace.WebApi.Startup
{
    public static class OpenApiConfig
    {
        public static void AddOpenApiServices(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<InfoSchemeTransformer>();
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });
        }

        public static void UseOpenApi(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference("/", options =>
                {
                    options.OpenApiRoutePattern = "/openapi/v1.json";
                    options.Title = "Orbit Space API V1";
                    options.Theme = ScalarTheme.BluePlanet;
                    options.ShowSidebar = true;
                    options.AddPreferredSecuritySchemes("Bearer");
                    options.SortOperationsByMethod();
                });
            }
        }
    }
}
