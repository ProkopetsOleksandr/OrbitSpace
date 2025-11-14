using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Dtos.TodoItem
{
    public record TodoItemDto(
    string Id,
    string Title,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    TodoItemStatus Status)
    {
        public string StatusDescription => Status.ToString();
    };
}