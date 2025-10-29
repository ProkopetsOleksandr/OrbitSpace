using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.Application.Models.Responses;

namespace OrbitSpace.Application.Interfaces.Services;

public interface ITodoItemService
{
    Task<List<TodoItemDto>> GetAllAsync(string userId);
    Task<OperationResult<TodoItemDto>> GetByIdAsync(string id);
    Task<OperationResult<TodoItemDto>> CreateAsync(CreateTodoItemDto todoItem);
    Task<OperationResult<TodoItemDto>> UpdateAsync(TodoItemDto todoItem);
    Task<OperationResult> DeleteAsync(string id);
}