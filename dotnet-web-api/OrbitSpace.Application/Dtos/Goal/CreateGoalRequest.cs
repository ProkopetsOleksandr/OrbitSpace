using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Dtos.Goal;

public record CreateGoalRequest(
    string Title,
    LifeArea LifeArea,
    bool IsActive,
    bool IsSmartGoal,
    string? Description,
    string? Metrics,
    string? AchievabilityRationale,
    string? Motivation,
    DateTime? DueDate);
