using OrbitSpace.WebApi.Models.Responses.Base;

namespace OrbitSpace.WebApi.Models.Responses.TodoItems
{
    public sealed record TodoItemResponse(TodoItemResource Data) : ApiSuccessResponse<TodoItemResource>(Data);
}
