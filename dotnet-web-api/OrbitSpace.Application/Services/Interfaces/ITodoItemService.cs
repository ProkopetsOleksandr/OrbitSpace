using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.TodoItem;

namespace OrbitSpace.Application.Services.Interfaces;

public interface ITodoItemService
{
    Task<OperationResult<TodoItemDto>> GetByIdAsync(Guid id, Guid userId);
    Task<List<TodoItemDto>> GetAllAsync(Guid userId);
    Task<OperationResult<TodoItemDto>> CreateAsync(CreateTodoItemDto todoItem, Guid userId);
    Task<OperationResult<TodoItemDto>> UpdateAsync(UpdateTodoItemDto request, Guid userId);
    Task<OperationResult> DeleteAsync(Guid id, Guid userId);
}
