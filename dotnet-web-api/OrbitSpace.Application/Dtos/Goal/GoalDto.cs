using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Dtos.Goal
{
    public record GoalDto(
        Guid Id,
        string Title,
        LifeArea LifeArea,
        GoalStatus Status,
        DateTime CreatedAtUtc,
        DateTime? CompletedAtUtc,
        DateTime? DueAtUtc,
        bool IsSmartGoal
    );
}
