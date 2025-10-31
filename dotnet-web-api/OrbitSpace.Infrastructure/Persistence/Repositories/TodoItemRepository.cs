using MongoDB.Driver;
using OrbitSpace.Application.Interfaces.Repositories;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class TodoItemRepository(IMongoDbContext dbContext) : ITodoItemRepository
{
    public async Task<List<TodoItem>> GetAllAsync(string userId)
    {
        var filter = Builders<TodoItem>.Filter.Eq(x => x.UserId, userId);
        
        return await dbContext.TodoItems.Find(filter).ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(string id)
    {
        var filter = Builders<TodoItem>.Filter.Eq(x => x.Id, id);
        
        return await dbContext.TodoItems.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem)
    {
        await dbContext.TodoItems.InsertOneAsync(todoItem);
        
        return todoItem;
    }

    public async Task<bool> UpdateAsync(TodoItem todoItem)
    {
        var filter = Builders<TodoItem>.Filter.Eq(x => x.Id, todoItem.Id);
        var updateResult = await dbContext.TodoItems.ReplaceOneAsync(filter, todoItem);
        
        return updateResult.IsAcknowledged;
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var filter = Builders<TodoItem>.Filter.And(
        Builders<TodoItem>.Filter.Eq(x => x.Id, id),
            Builders<TodoItem>.Filter.Eq(x => x.UserId, userId));
        
        var deleteResult = await dbContext.TodoItems.DeleteOneAsync(filter);

        return deleteResult.DeletedCount > 0;
    }
}