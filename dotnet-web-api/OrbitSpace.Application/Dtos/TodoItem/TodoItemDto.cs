using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Dtos.TodoItem
{
    public record TodoItemDto(
    string Id,
    string Title,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    TodoItemStatus Status);
}