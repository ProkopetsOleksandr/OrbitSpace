using MongoDB.Bson.Serialization.Conventions;

using OrbitSpace.Infrastructure.Persistence.Maps;

namespace OrbitSpace.Infrastructure.Persistence;

public static class MongoMappingsConfig
{
    public static void RegisterAll()
    {
        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention()
        };

        ConventionRegistry.Register("AppConventions", conventionPack, t => true); // Apply to all classes


        UserMap.Configure();
        TodoItemMap.Configure();
        GoalMap.Configure();
        ActivityMap.Configure();
    }
}