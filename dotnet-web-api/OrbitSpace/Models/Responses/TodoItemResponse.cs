using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.WebApi.Models.Responses.Base;

namespace OrbitSpace.WebApi.Models.Responses
{
    public sealed record TodoItemResponse(TodoItemDto Data) : ApiSuccessResponse<TodoItemDto>(Data);
}
