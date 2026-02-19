using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Domain.Entities;

public class TodoItem
{
    public required Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Title { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public TodoItemStatus Status { get; set; }
}
