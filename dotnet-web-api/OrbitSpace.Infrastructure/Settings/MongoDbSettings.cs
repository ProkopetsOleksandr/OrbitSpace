namespace OrbitSpace.Infrastructure.Settings
{
    public class MongoDbSettings
    {
        public const string SectionName = "MongoDbSettings";
        public required string ConnectionString { get; init; }
        public required string DatabaseName { get; init; }
    }
}
