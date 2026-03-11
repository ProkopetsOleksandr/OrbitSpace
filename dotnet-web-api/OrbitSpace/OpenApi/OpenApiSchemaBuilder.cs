namespace OrbitSpace.WebApi.OpenApi
{
    public class OpenApiSchemaBuilder<T>(OpenApiSchemaMetadata mapping)
    {
        public OpenApiSchemaBuilder<T> WithSchemaName(string schemaName)
        {
            mapping.SchemaName = schemaName;
            return this;
        }

        public OpenApiSchemaBuilder<T> WithTitle(string title)
        {
            mapping.Title = title;
            return this;
        }

        public OpenApiSchemaBuilder<T> WithDescription(string description)
        {
            mapping.Description = description;
            return this;
        }

        public OpenApiSchemaBuilder<T> WithExample(T example)
        {
            mapping.Example = example;
            return this;
        }
    }
}
