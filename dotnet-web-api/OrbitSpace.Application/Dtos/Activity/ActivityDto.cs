namespace OrbitSpace.Application.Dtos.Activity
{
    public record ActivityDto(
        string Id,
        string Name,
        string Code,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );
}
