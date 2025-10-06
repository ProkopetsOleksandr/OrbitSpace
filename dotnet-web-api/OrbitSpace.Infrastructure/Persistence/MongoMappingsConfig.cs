using OrbitSpace.Infrastructure.Persistence.Maps;

namespace OrbitSpace.Infrastructure.Persistence;

public static class MongoMappingsConfig
{
    public static void RegisterAll()
    {
        UserMap.Configure();
    }
}