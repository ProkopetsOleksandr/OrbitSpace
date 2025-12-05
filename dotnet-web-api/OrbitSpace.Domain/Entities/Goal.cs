using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Domain.Entities
{
    public class Goal
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public LifeArea LifeArea { get;set; }
        public GoalStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DateTime? StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public DateTime? CancelledAtUtc { get; set; }

        // Smart Goal Attributes
        public bool IsSmartGoal { get; set; }
        public string? Description { get; set; }
        public string? Metrics { get; set; }
        public string? AchievabilityRationale { get; set; }
        public string? Motivation { get; set; }
        public DateTime? DueAtUtc { get; set; }
    }
}
