using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.TodoItem;

namespace OrbitSpace.Application.Services.Interfaces;

public interface ITodoItemService
{
    Task<List<TodoItemDto>> GetAllAsync(string userId);
    Task<TodoItemDto?> GetByIdAsync(string id, string userId);
    Task<OperationResult<TodoItemDto>> CreateAsync(CreateTodoItemDto todoItem, string userId);
    Task<OperationResult<TodoItemDto>> UpdateAsync(TodoItemDto todoItem, string userId);
    Task<bool> DeleteAsync(string id, string userId);
}  