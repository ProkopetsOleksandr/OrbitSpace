using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class TodoItemRepository(AppDbContext dbContext) : ITodoItemRepository
{
    public async Task<List<TodoItem>> GetAllAsync(Guid userId)
    {
        return await dbContext.TodoItems
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id)
    {
        return await dbContext.TodoItems.FindAsync(id);
    }

    public async Task<TodoItem> CreateAsync(TodoItem todoItem)
    {
        dbContext.TodoItems.Add(todoItem);
        await dbContext.SaveChangesAsync();
        return todoItem;
    }

    public async Task<bool> UpdateAsync(TodoItem todoItem)
    {
        dbContext.TodoItems.Update(todoItem);
        var affected = await dbContext.SaveChangesAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var affected = await dbContext.TodoItems
            .Where(t => t.Id == id && t.UserId == userId)
            .ExecuteDeleteAsync();
        return affected > 0;
    }
}
