using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace OrbitSpace.WebApi.OpenApi;

public class InfoSchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
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
    }
}