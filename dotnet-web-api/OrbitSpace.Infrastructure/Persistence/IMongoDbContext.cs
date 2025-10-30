using MongoDB.Driver;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence;

public interface IMongoDbContext
{
    IMongoCollection<User> Users { get; }
    IMongoCollection<TodoItem> TodoItems { get; }
}