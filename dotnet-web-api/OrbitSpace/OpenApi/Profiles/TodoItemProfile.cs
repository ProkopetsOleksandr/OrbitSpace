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
                .WithExample(new TodoItemDto(
                    Id: "679c2e4fa7b19f34d2c8f91b",
                    Title: "Example Title",
                    CreatedAt: new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc),
                    UpdatedAt: new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc),
                    Status: TodoItemStatus.New));

            CreateSchema<CreateTodoItemDto>()
                .WithSchemaName("CreateTodoItemPayload")
                .WithDescription("Model used to create todo items")
                .WithExample(new CreateTodoItemDto(
                    Title: "Example Title for Creation"));

            CreateSchema<UpdateTodoItemDto>()
                .WithSchemaName("UpdateTodoItemPayload")
                .WithExample(new UpdateTodoItemDto(
                    Id: "679c2e4fa7b19f34d2c8f91b",
                    Title: "New title",
                    Status: TodoItemStatus.InProgress));
        }
    }
}
