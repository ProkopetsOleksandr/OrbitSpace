namespace OrbitSpace.Application.Dtos.Activity
{
    public record UpdateActivityRequest(
        string Id,
        string Name,
        string Code
    );
}
