using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Domain.Entities;

public class TodoItem
{
    public string? Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public TodoItemStatus Status { get; set; }
}