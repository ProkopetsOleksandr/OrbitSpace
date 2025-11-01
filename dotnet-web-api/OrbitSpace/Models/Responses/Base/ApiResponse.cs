namespace OrbitSpace.WebApi.Models.Responses.Base
{
    public abstract record ApiResponse
    {
        public ApiMeta Meta { get; init; } = new();
    }
}
