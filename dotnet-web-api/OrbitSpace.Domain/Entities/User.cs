using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrbitSpace.Domain.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        // Никогда не храните пароль в открытом виде! Только хеш.
        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
