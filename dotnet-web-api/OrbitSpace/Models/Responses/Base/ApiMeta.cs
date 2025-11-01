namespace OrbitSpace.WebApi.Models.Responses.Base
{
    public record ApiMeta
    {
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
