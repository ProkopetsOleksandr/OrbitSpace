using OrbitSpace.Application.Dtos.TodoItem;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.WebApi.OpenApi.Profiles
{
    public class TodoItemProfile : OpenApiSchemaProfile
    {
        public TodoItemProfile()
        {
            CreateSchema<TodoItemDto>()
                .WithTitle("TodoItem")
                .WithDescription("Represents a Todo item")
                .WithExample(new TodoItemDto("1", "Example Title", DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow, TodoItemStatus.New));

            CreateSchema<CreateTodoItemDto>()
                .WithTitle("CreateTodoItemPayload")
                .WithDescription("Model used to create todo items")
                .WithExample(new CreateTodoItemDto("Example Title for Creation"));

            CreateSchema<UpdateTodoItemDto>()
                .WithTitle("UpdateTodoItemPayload")
                .WithExample(new UpdateTodoItemDto("1", "New title", TodoItemStatus.InProgress));
        }
    }
}
