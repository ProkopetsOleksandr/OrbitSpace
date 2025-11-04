using OrbitSpace.Domain.Enums;

namespace OrbitSpace.WebApi.Models.Responses.TodoItems
{
    public record TodoItemResource(
        string Id,
        string Title,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        TodoItemStatus Status,
        string StatusDescription);
}
