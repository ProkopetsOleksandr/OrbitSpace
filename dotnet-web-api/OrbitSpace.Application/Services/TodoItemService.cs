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

    public async Task<OperationResult<TodoItemDto>> GetByIdAsync(string id, string userId)
    {
        var item = await GetByIdForUserAsync(id, userId);
        if (item == null)
        {
            return OperationResultError.NotFound();
        }
        
        return MapToDto(item);
    }

    public async Task<OperationResult<TodoItemDto>> CreateAsync(CreateTodoItemDto todoItem, string userId)
    {
        if (string.IsNullOrWhiteSpace(todoItem.Title))
        {
            return OperationResultError.Validation("Title is required");
        }
        
        var currentDateTime = DateTime.UtcNow;
        var created = await todoItemRepository.CreateAsync(new TodoItem
        {
            UserId = userId,
            Title = todoItem.Title,
            CreatedAt = currentDateTime,
            UpdatedAt = currentDateTime,
            Status = TodoItemStatus.New
        });
        
        return MapToDto(created);
    }

    public async Task<OperationResult<TodoItemDto>> UpdateAsync(TodoItemDto todoItem, string userId)
    {
        var entityInDb = await GetByIdForUserAsync(todoItem.Id, userId);
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

    public async Task<OperationResult> DeleteAsync(string id, string userId)
    {
        if (!await todoItemRepository.DeleteAsync(id, userId))
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
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            Status = item.Status,
            StatusDescription = item.Status.ToString()
        };
    }

    private async Task<TodoItem?> GetByIdForUserAsync(string id, string userId)
    {
        var entityInDb = await todoItemRepository.GetByIdAsync(id);
        if (entityInDb == null || entityInDb.UserId != userId)
        {
            return null;
        }

        return entityInDb;
    }
}