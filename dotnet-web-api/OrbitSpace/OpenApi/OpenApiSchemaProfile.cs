namespace OrbitSpace.WebApi.OpenApi
{
    public abstract class OpenApiSchemaProfile
    {
        private readonly List<OpenApiSchemaMetadata> _mappings = new();

        protected OpenApiSchemaBuilder<T> CreateSchema<T>()
        {
            var m = new OpenApiSchemaMetadata(typeof(T));
            _mappings.Add(m);
            return new OpenApiSchemaBuilder<T>(m);
        }

        public IEnumerable<OpenApiSchemaMetadata> GetMappings() => _mappings;
    }
}