namespace OrbitSpace.WebApi.OpenApi
{
    public class OpenApiSchemaMetadata
    {
        public Type Type { get; }
        
        public string SchemaName
        {
            get => field ?? Type.ToString();
            set { field = value; }
        }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public object? Example { get; set; }

        public OpenApiSchemaMetadata(Type type)
        {
            Type = type;
        }
    }
}
