using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Models.Dtos.Todo;

public class TodoItemDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public TodoItemStatus Status { get; set; }
    public string StatusDescription { get; set; } = string.Empty;
}