using System.Collections;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.OpenApi;
using OrbitSpace.WebApi.Models.Responses;

namespace OrbitSpace.WebApi.OpenApi
{
    public static class OpenApiSchemaReferenceIdGenerator
    {
        public static string? GetSchemaName(JsonTypeInfo typeInfo, IReadOnlyDictionary<Type, OpenApiSchemaMetadata> schemaMetadataConfig)
        {
            var type = typeInfo.Type;

            if (schemaMetadataConfig.ContainsKey(type))
            {
                return schemaMetadataConfig[type].SchemaName;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                var responseType = type.GetGenericArguments()[0];
                var isCollection = typeof(IEnumerable).IsAssignableFrom(responseType);
                var dataType = isCollection ? GetCollectionDataType(responseType) : responseType;

                var schemaName = schemaMetadataConfig.ContainsKey(dataType)
                    ? schemaMetadataConfig[dataType].SchemaName
                    : dataType.Name;

                return schemaName + (isCollection ? "sResponse" : "Response");
            }

            return OpenApiOptions.CreateDefaultSchemaReferenceId(typeInfo);
        }

        private static Type GetCollectionDataType(Type collectionType)
        {
            var collectionDataType = collectionType
                .GetInterfaces()
                .Concat([collectionType])
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return collectionDataType?.GetGenericArguments()[0] ?? collectionType;
        }
    }
}
