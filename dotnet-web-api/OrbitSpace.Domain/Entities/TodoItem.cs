using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Domain.Entities;

public class TodoItem
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public TodoItemStatus Status { get; set; }
}