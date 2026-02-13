using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.TodoItem;

namespace OrbitSpace.Application.Services.Interfaces;

public interface ITodoItemService
{
    Task<List<TodoItemDto>> GetAllAsync(Guid userId);
    Task<TodoItemDto?> GetByIdAsync(Guid id, Guid userId);
    Task<OperationResult<TodoItemDto>> CreateAsync(CreateTodoItemDto todoItem, Guid userId);
    Task<OperationResult<TodoItemDto>> UpdateAsync(UpdateTodoItemDto todoItem, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
