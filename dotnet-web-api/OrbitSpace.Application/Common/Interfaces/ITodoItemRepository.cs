using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface ITodoItemRepository
    {
        Task<List<TodoItem>> GetAllAsync(Guid userId);
        Task<TodoItem?> GetByIdAsync(Guid id);
        Task<TodoItem> CreateAsync(TodoItem todoItem);
        Task<bool> UpdateAsync(TodoItem todoItem);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
