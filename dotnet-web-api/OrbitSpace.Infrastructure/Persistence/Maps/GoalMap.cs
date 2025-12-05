using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Maps
{
    public static class GoalMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<Goal>(cm =>
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

                cm.MapMember(m => m.StartedAtUtc)
                    .SetElementName("startedAt")
                    .SetSerializer(new NullableSerializer<DateTime>(
                        new DateTimeSerializer(DateTimeKind.Utc)));

                cm.MapMember(m => m.CompletedAtUtc)
                    .SetElementName("completedAt")
                    .SetSerializer(new NullableSerializer<DateTime>(
                        new DateTimeSerializer(DateTimeKind.Utc)));

                cm.MapMember(m => m.CancelledAtUtc)
                    .SetElementName("cancelledAt")
                    .SetSerializer(new NullableSerializer<DateTime>(
                        new DateTimeSerializer(DateTimeKind.Utc)));

                cm.MapMember(m => m.DueAtUtc)
                    .SetElementName("dueAt")
                    .SetSerializer(new NullableSerializer<DateTime>(
                        new DateTimeSerializer(DateTimeKind.Utc)));
            });
        }
    }
}
