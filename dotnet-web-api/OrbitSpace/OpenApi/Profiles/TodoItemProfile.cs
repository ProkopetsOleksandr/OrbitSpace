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
                    Id: new Guid("019c577c-c280-7103-ad24-9045db19c9fe"),
                    Title: "Example Title",
                    CreatedAtUtc: new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc),
                    UpdatedAtUtc: new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc),
                    Status: TodoItemStatus.New));

            CreateSchema<CreateTodoItemDto>()
                .WithSchemaName("CreateTodoItemPayload")
                .WithDescription("Model used to create todo items")
                .WithExample(new CreateTodoItemDto(
                    Title: "Example Title for Creation"));

            CreateSchema<UpdateTodoItemDto>()
                .WithSchemaName("UpdateTodoItemPayload")
                .WithExample(new UpdateTodoItemDto(
                    Id: new Guid("019c577c-c280-7103-ad24-9045db19c9fe"),
                    Title: "New title",
                    Status: TodoItemStatus.InProgress));
        }
    }
}
