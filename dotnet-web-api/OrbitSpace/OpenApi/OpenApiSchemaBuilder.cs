namespace OrbitSpace.WebApi.OpenApi
{
    public class OpenApiSchemaBuilder<T>
    {
        private readonly OpenApiSchemaMetadata _mapping;

        public OpenApiSchemaBuilder(OpenApiSchemaMetadata mapping)
        {
            _mapping = mapping;
        }

        public OpenApiSchemaBuilder<T> WithTitle(string title)
        {
            _mapping.Title = title;
            return this;
        }

        public OpenApiSchemaBuilder<T> WithDescription(string description)
        {
            _mapping.Description = description;
            return this;
        }

        public OpenApiSchemaBuilder<T> WithExample(T example)
        {
            _mapping.Example = example;
            return this;
        }
    }
}
