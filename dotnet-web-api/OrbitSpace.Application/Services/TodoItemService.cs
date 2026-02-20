using MapsterMapper;
using OrbitSpace.Application.Common.Interfaces;
using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Dtos.TodoItem;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Domain.Entities;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Services
{
    public class TodoItemService(IUnitOfWork unitOfWork, ITodoItemRepository todoItemRepository, IMapper mapper) : ITodoItemService
    {
        public async Task<OperationResult<TodoItemDto>> GetByIdAsync(Guid id, Guid userId)
        {
            var todoItem = await todoItemRepository.FindByIdAsync(id, userId);
            
            return todoItem == null ? OperationResultError.NotFound()  : mapper.Map<TodoItemDto>(todoItem);
        }
        
        public async Task<List<TodoItemDto>> GetAllAsync(Guid userId)
        {
            var items = await todoItemRepository.GetAllAsync(userId);

            return mapper.Map<List<TodoItemDto>>(items);
        }

        public async Task<OperationResult<TodoItemDto>> CreateAsync(CreateTodoItemDto request, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return OperationResultError.Validation("Title is required");
            }

            var currentDateTime = DateTime.UtcNow;
            var todoItem = new TodoItem
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Title = request.Title,
                CreatedAtUtc = currentDateTime,
                UpdatedAtUtc = currentDateTime,
                Status = TodoItemStatus.New
            };
            
            todoItemRepository.Add(todoItem);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<TodoItemDto>(todoItem);
        }

        public async Task<OperationResult<TodoItemDto>> UpdateAsync(UpdateTodoItemDto request, Guid userId)
        {
            var todoItem = await todoItemRepository.FindByIdAsync(request.Id, userId);
            if (todoItem == null)
            {
                return OperationResultError.NotFound();
            }

            todoItem.Title = request.Title;
            todoItem.Status = request.Status;
            todoItem.UpdatedAtUtc = DateTime.UtcNow;

            todoItemRepository.Update(todoItem);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<TodoItemDto>(todoItem);
        }

        public async Task<OperationResult> DeleteAsync(Guid id, Guid userId)
        {
            var affected = await todoItemRepository.DeleteAsync(id, userId);
            
            return affected == 0 ? OperationResultError.NotFound() : OperationResult.Success();
        }
    }
}
