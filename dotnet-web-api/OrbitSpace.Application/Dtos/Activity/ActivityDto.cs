namespace OrbitSpace.Application.Dtos.Activity
{
    public record ActivityDto(
        Guid Id,
        string Name,
        string Code,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );
}
