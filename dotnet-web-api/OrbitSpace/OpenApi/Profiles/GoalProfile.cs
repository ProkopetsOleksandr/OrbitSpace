using OrbitSpace.Application.Dtos.Goal;
using OrbitSpace.Domain.Enums;

namespace OrbitSpace.WebApi.OpenApi.Profiles
{
    public class GoalProfile : OpenApiSchemaProfile
    {
        public GoalProfile()
        {
            CreateSchema<GoalDto>()
                .WithSchemaName("Goal")
                .WithDescription("Represents a Goal")
                .WithExample(new GoalDto(new Guid().ToString(), "Learn Aspire", LifeArea.Career, GoalStatus.NotStarted, new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 3, 17, 55, 0, DateTimeKind.Utc)));
        }
    }
}
