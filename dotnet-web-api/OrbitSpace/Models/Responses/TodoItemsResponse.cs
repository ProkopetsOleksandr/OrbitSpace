using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.WebApi.Models.Responses.Base;

namespace OrbitSpace.WebApi.Models.Responses
{
    public sealed record TodoItemsResponse(IEnumerable<TodoItemDto> Data) : ApiSuccessResponse<IEnumerable<TodoItemDto>>(Data);
}
