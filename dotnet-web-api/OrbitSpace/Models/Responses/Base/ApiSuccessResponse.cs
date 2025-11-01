namespace OrbitSpace.WebApi.Models.Responses.Base
{
    public record ApiSuccessResponse<T>(T Data) : ApiResponse where T : notnull;
}
