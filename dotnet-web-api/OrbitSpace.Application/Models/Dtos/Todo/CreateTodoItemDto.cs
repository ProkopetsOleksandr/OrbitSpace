namespace OrbitSpace.Application.Models.Dtos.Todo;

public class CreateTodoItemDto
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}