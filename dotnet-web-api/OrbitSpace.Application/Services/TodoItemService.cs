using OrbitSpace.Application.Interfaces.Repositories;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.Application.Models.Responses;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Services;

public class TodoItemService(ITodoItemRepository todoItemRepository) : ITodoItemService
{
    public async Task<List<TodoItemDto>> GetAllAsync(string userId)
    {
        var items = await todoItemRepository.GetAllAsync(userId);
        
        return items.Select(MapToDto).ToList();
    }

    public async Task<OperationResult<TodoItemDto>> GetByIdAsync(string id)
    {
        var item = await todoItemRepository.GetByIdAsync(id);
        if (item == null)
        {
            return OperationResultError.NotFound();
        }
        
        return MapToDto(item);
    }

    public async Task<OperationResult<TodoItemDto>> CreateAsync(CreateTodoItemDto todoItem)
    {
        if (string.IsNullOrWhiteSpace(todoItem.Title))
        {
            return OperationResultError.Validation("Title is required");
        }
        
        var currentDateTime = DateTime.UtcNow;
        var created = await todoItemRepository.CreateAsync(new TodoItem
        {
            UserId = todoItem.UserId,
            Title = todoItem.Title,
            CreatedAt = currentDateTime,
            UpdatedAt = currentDateTime,
            Status = TodoItemStatus.New
        });
        
        return MapToDto(created);
    }

    public async Task<OperationResult<TodoItemDto>> UpdateAsync(TodoItemDto todoItem)
    {
        var entityInDb = await todoItemRepository.GetByIdAsync(todoItem.Id);
        if (entityInDb == null)
        {
            return OperationResultError.NotFound();
        }

        entityInDb.Title = todoItem.Title;
        entityInDb.Status = todoItem.Status;
        entityInDb.UpdatedAt = DateTime.UtcNow;
            
        await todoItemRepository.UpdateAsync(entityInDb);
        
        return MapToDto(entityInDb);
    }

    public async Task<OperationResult> DeleteAsync(string id)
    {
        if (!await todoItemRepository.DeleteAsync(id))
        {
            return OperationResultError.NotFound();
        }
        
        return OperationResult.Success();
    }
    
    private static TodoItemDto MapToDto(TodoItem item)
    {
        return new TodoItemDto
        {
            Id = item.Id!,
            Title = item.Title,
            UserId = item.UserId,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            Status = item.Status,
            StatusDescription = item.Status.ToString()
        };
    }
}