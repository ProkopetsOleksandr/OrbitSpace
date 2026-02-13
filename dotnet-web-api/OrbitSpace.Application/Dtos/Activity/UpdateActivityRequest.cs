namespace OrbitSpace.Application.Dtos.Activity
{
    public record UpdateActivityRequest(
        Guid Id,
        string Name,
        string Code
    );
}
