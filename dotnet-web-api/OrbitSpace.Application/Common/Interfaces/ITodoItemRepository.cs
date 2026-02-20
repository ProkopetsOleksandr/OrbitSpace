using OrbitSpace.Domain.Entities;

namespace OrbitSpace.Application.Common.Interfaces
{
    public interface ITodoItemRepository
    {
        Task<TodoItem?> FindByIdAsync(Guid id, Guid userId);
        Task<List<TodoItem>> GetAllAsync(Guid userId);
        void Add(TodoItem todoItem);
        void Update(TodoItem todoItem);
        Task<int> DeleteAsync(Guid id, Guid userId);
    }
}
