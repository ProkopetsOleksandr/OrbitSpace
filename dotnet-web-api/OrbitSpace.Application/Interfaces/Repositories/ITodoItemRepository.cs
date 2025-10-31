using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Interfaces.Repositories;

public interface ITodoItemRepository
{
    Task<List<TodoItem>> GetAllAsync(string userId);
    Task<TodoItem?> GetByIdAsync(string id);
    Task<TodoItem> CreateAsync(TodoItem todoItem);
    Task<bool> UpdateAsync(TodoItem todoItem);
    Task<bool> DeleteAsync(string id, string userId);
}