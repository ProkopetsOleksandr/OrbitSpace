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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? CancelledDate { get; set; }

        // Smart Goal Attributes
        public string? Description { get; set; }
        public string? Metrics { get; set; }
        public string? AchievabilityRationale { get; set; }
        public string? Motivation { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
