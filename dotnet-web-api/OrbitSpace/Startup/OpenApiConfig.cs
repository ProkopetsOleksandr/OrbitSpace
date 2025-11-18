using System;
using System.Collections;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OrbitSpace.WebApi.Models.Responses;
using OrbitSpace.WebApi.OpenApi;
using OrbitSpace.WebApi.OpenApi.DocumentTransformers;
using OrbitSpace.WebApi.OpenApi.OperationTransformers;
using OrbitSpace.WebApi.OpenApi.SchemaTransformers;
using Scalar.AspNetCore;

namespace OrbitSpace.WebApi.Startup
{
    public static class OpenApiConfig
    {
        public static void AddOpenApiServices(this IServiceCollection services)
        {
            var schemaMetadataConfig = OpenApiConfigurator.GetSchemaMetadataConfig();

            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info.Title = "Orbit Space API";
                    document.Info.Version = "v1";
                    document.Info.Description = "API for processing data from the Next.js project";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "Oleksandr Prokopets",
                        Url = new Uri("https://github.com/prokopetsoleksandr")
                    };

                    return Task.CompletedTask;
                });

                options.CreateSchemaReferenceId = typeInfo =>
                {
                    return OpenApiSchemaReferenceIdGenerator.GetSchemaName(typeInfo, schemaMetadataConfig);
                };

                options.AddDocumentTransformer<BearerSecuritySchemeDocumentTransformer>();
                options.AddSchemaTransformer(new OpenApiMetadataSchemaTransformer(schemaMetadataConfig));
                options.AddOperationTransformer<BearerSecurityRequirementOperationTransformer>();
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
                    options.Theme = ScalarTheme.BluePlanet;
                    options.ShowSidebar = true;
                    options.AddPreferredSecuritySchemes("Bearer");
                    options.SortOperationsByMethod();
                });
            }
        }
    }
}
