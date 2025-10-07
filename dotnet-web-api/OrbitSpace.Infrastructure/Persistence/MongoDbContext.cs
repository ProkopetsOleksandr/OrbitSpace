﻿using MongoDB.Driver;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }
    
    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
}