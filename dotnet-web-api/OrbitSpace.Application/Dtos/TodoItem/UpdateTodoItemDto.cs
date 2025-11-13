using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Dtos.TodoItem
{
    public record UpdateTodoItemDto(string Id, string Title, TodoItemStatus Status);
}
