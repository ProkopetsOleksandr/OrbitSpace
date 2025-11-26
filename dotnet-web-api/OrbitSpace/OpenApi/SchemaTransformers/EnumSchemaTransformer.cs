using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OrbitSpace.WebApi.OpenApi.SchemaTransformers;

public class EnumSchemaTransformer: IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var type = context.JsonTypeInfo.Type;
        if (type.IsEnum)
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = null;
            schema.Enum ??= new List<JsonNode>();
            schema.Enum.Clear();

            foreach (var name in Enum.GetNames(type))
            {
                schema.Enum.Add(JsonValue.Create(name));
            }
        }

        return Task.CompletedTask;
    }
}