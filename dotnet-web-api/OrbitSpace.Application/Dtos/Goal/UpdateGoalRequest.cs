using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Application.Dtos.Goal;

public record UpdateGoalRequest(
    Guid Id,
    string Title,
    LifeArea LifeArea,
    GoalStatus Status,
    bool IsSmartGoal,
    string? Description,
    string? Metrics,
    string? AchievabilityRationale,
    string? Motivation,
    DateTime? DueDate);