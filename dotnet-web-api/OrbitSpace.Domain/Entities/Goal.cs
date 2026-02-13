using OrbitSpace.Domain.Enums;

namespace OrbitSpace.Domain.Entities
{
    public class Goal
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Title { get; set; }
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
