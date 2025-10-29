using MongoDB.Driver;
using OrbitSpace.Application.Interfaces.Repositories;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class TodoItemRepository(IMongoDatabase database) : ITodoItemRepository
{
    private readonly IMongoCollection<TodoItem> _todoItemsCollection = database.GetCollection<TodoItem>("todoItems");

    public async Task<List<TodoItem>> GetAllAsync(string userId)
    {
        var filter = Builders<TodoItem>.Filter.Eq(x => x.UserId, userId);
        
        return await _todoItemsCollection.Find(filter).ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(string id)
    {
        var filter = Builders<TodoItem>.Filter.Eq(x => x.Id, id);
        
        return await _todoItemsCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem)
    {
        await _todoItemsCollection.InsertOneAsync(todoItem);
        
        return todoItem;
    }

    public async Task<bool> UpdateAsync(TodoItem todoItem)
    {
        var filter = Builders<TodoItem>.Filter.Eq(x => x.Id, todoItem.Id);
        var updateResult = await _todoItemsCollection.ReplaceOneAsync(filter, todoItem);
        
        return updateResult.IsAcknowledged;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<TodoItem>.Filter.Eq(x => x.Id, id);
        var deleteResult = await _todoItemsCollection.DeleteOneAsync(filter);

        return deleteResult.DeletedCount > 0;
    }
}