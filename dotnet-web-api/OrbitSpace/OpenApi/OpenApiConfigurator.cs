using OrbitSpace.WebApi.OpenApi.Profiles;

namespace OrbitSpace.WebApi.OpenApi
{
    public static class OpenApiConfigurator
    {
        public static IReadOnlyDictionary<Type, OpenApiSchemaMetadata> GetSchemaMetadataConfig()
        {
            OpenApiSchemaProfile[] profiles = [
                new TodoItemProfile(),
                new GoalProfile(),
                new ActivityProfile()
            ];

            var config = new Dictionary<Type, OpenApiSchemaMetadata>();
            foreach (var p in profiles)
            {
                foreach (var mapping in p.GetMappings())
                {
                    config[mapping.Type] = mapping;
                }
            }

            return config;
        }
    }
}
