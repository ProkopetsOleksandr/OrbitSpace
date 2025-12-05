using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Maps;

public static class TodoItemMap
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<TodoItem>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(m => m.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            
            cm.MapMember(m => m.CreatedAtUtc)
                .SetElementName("createdAt")
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));

            cm.MapMember(m => m.UpdatedAtUtc)
                .SetElementName("updatedAt")
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
        });
    }
}