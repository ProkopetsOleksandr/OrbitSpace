using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Models.Dtos.Todo
{
    public record TodoItemDto(
    string Id,
    string Title,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    TodoItemStatus Status,
    string StatusDescription);
}