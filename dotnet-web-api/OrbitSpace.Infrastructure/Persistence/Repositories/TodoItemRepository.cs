using Microsoft.EntityFrameworkCore;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Infrastructure.Persistence.Repositories;

public class TodoItemRepository(AppDbContext dbContext) : ITodoItemRepository
{
    public async Task<TodoItem?> FindByIdAsync(Guid id, Guid userId)
    {
        return await dbContext.TodoItems.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
    }
    
    public async Task<List<TodoItem>> GetAllAsync(Guid userId)
    {
        return await dbContext.TodoItems
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public void Add(TodoItem todoItem)
    {
        dbContext.TodoItems.Add(todoItem);
    }

    public void Update(TodoItem todoItem)
    {
        dbContext.TodoItems.Update(todoItem);
    }

    public async Task<int> DeleteAsync(Guid id, Guid userId)
    {
        return await dbContext.TodoItems
            .Where(t => t.Id == id && t.UserId == userId)
            .ExecuteDeleteAsync();
    }
}
