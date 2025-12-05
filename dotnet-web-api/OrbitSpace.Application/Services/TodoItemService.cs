using MapsterMapper;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.TodoItem;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Services
{
    public class TodoItemService(ITodoItemRepository todoItemRepository, IMapper mapper) : ITodoItemService
    {
        public async Task<List<TodoItemDto>> GetAllAsync(string userId)
        {
            var items = await todoItemRepository.GetAllAsync(userId);

            return mapper.Map<List<TodoItemDto>>(items);
        }

        public async Task<TodoItemDto?> GetByIdAsync(string id, string userId)
        {
            var item = await GetByIdForUserAsync(id, userId);
            
            return item == null ? null : mapper.Map<TodoItemDto>(item);
        }

        public async Task<OperationResult<TodoItemDto>> CreateAsync(CreateTodoItemDto todoItem, string userId)
        {
            if (string.IsNullOrWhiteSpace(todoItem.Title))
            {
                return OperationResultError.Validation("Title is required");
            }

            var currentDateTime = DateTime.UtcNow;
            var createdItem = await todoItemRepository.CreateAsync(new TodoItem
            {
                UserId = userId,
                Title = todoItem.Title,
                CreatedAtUtc = currentDateTime,
                UpdatedAtUtc = currentDateTime,
                Status = TodoItemStatus.New
            });

            return mapper.Map<TodoItemDto>(createdItem);
        }

        public async Task<OperationResult<TodoItemDto>> UpdateAsync(UpdateTodoItemDto todoItem, string userId)
        {
            var entityInDb = await GetByIdForUserAsync(todoItem.Id, userId);
            if (entityInDb == null)
            {
                return OperationResultError.NotFound();
            }

            entityInDb.Title = todoItem.Title;
            entityInDb.Status = todoItem.Status;
            entityInDb.UpdatedAtUtc = DateTime.UtcNow;

            await todoItemRepository.UpdateAsync(entityInDb);

            return mapper.Map<TodoItemDto>(entityInDb);
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            return await todoItemRepository.DeleteAsync(id, userId);
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
}