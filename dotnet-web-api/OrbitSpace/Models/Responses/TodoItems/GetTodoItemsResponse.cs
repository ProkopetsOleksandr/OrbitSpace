using OrbitSpace.WebApi.Models.Responses.Base;

namespace OrbitSpace.WebApi.Models.Responses.TodoItems
{
    public sealed record GetTodoItemsResponse(IEnumerable<TodoItemResource> Data) : ApiSuccessResponse<IEnumerable<TodoItemResource>>(Data);
}
