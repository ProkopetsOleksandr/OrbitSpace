using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.Application.Models.Responses;

namespace OrbitSpace.Application.Interfaces.Services;

public interface ITodoItemService
{
    Task<List<TodoItemDto>> GetAllAsync(string userId);
    Task<OperationResult<TodoItemDto>> GetByIdAsync(string id, string userId);
    Task<OperationResult<TodoItemDto>> CreateAsync(CreateTodoItemDto todoItem, string userId);
    Task<OperationResult<TodoItemDto>> UpdateAsync(TodoItemDto todoItem, string userId);
    Task<OperationResult> DeleteAsync(string id, string userId);
}