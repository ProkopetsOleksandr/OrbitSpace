namespace OrbitSpace.WebApi.Models.Responses.Base
{
    public sealed record ApiErrorResponse(ApiError Error) : ApiResponse;
}
