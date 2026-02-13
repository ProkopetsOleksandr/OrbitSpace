using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Dtos.TodoItem
{
    public record UpdateTodoItemDto(Guid Id, string Title, TodoItemStatus Status);
}
