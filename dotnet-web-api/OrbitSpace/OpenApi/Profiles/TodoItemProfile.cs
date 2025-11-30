using OrbitSpace.Application.Dtos.TodoItem;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.WebApi.OpenApi.Profiles
{
    public class TodoItemProfile : OpenApiSchemaProfile
    {
        public TodoItemProfile()
        {
            CreateSchema<TodoItemDto>()
                .WithSchemaName("TodoItem")
                .WithDescription("Represents a Todo item")
                .WithExample(new TodoItemDto("1", "Example Title", new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc), new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc), TodoItemStatus.New));

            CreateSchema<CreateTodoItemDto>()
                .WithSchemaName("CreateTodoItemPayload")
                .WithDescription("Model used to create todo items")
                .WithExample(new CreateTodoItemDto("Example Title for Creation"));

            CreateSchema<UpdateTodoItemDto>()
                .WithSchemaName("UpdateTodoItemPayload")
                .WithExample(new UpdateTodoItemDto("1", "New title", TodoItemStatus.InProgress));
        }
    }
}
