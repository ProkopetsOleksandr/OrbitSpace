using OrbitSpace.Application.Dtos.TodoItem;
using OrbitSpace.WebApi.Models.Responses.Base;

namespace OrbitSpace.WebApi.Models.Responses.TodoItems
{
    public sealed record GetTodoItemsResponse(IEnumerable<TodoItemDto> Data) : ApiSuccessResponse<IEnumerable<TodoItemDto>>(Data);
}
