using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OrbitSpace.WebApi.OpenApi.SchemaTransformers
{
    public class OpenApiMetadataSchemaTransformer(IReadOnlyDictionary<Type, OpenApiSchemaMetadata> config) : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
        {
            if (config.TryGetValue(context.JsonTypeInfo.Type, out var m))
            {
                if (m.Title != null)
                {
                    schema.Title = m.Title;
                }

                if (m.Description != null)
                {
                    schema.Description = m.Description;
                }

                if (m.Example != null)
                {
                    schema.Example = JsonNode.Parse(JsonSerializer.Serialize(m.Example));
                }
            }

            return Task.CompletedTask;
        }
    }
}
