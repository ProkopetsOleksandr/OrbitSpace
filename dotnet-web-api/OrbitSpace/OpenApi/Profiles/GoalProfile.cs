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
                .WithExample(new GoalDto(new Guid().ToString(), "Learn Aspire", LifeArea.Career, GoalStatus.NotStarted, DateTime.UtcNow, null, DateTime.UtcNow.AddMonths(3)));
        }
    }
}
