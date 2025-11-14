namespace OrbitSpace.WebApi.Models.Responses
{
    public record ApiResponse<T>(T Data) where T : notnull;
}
