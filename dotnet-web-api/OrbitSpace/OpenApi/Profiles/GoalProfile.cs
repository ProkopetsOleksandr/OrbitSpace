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
                .WithExample(new GoalDto(
                    Id: "678a4f92c1a3b05f447e12f9",
                    Title: "Learn Aspire",
                    LifeArea: LifeArea.Career,
                    Status: GoalStatus.NotStarted,
                    CreatedAt: new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc),
                    CompletedDate: null,
                    DueDate: new DateTime(2026, 3, 3, 17, 55, 0, DateTimeKind.Utc)));

            CreateSchema<CreateGoalRequest>()
                .WithSchemaName("CreateGoalPayload")
                .WithDescription("Model used to create a goal")
                .WithExample(new CreateGoalRequest(
                    Title: "Build and publish a full-stack side project",
                    LifeArea: LifeArea.Career,
                    IsActive: false,
                    IsSmartGoal: true,
                    Description: "Create a complete full-stack web application to strengthen my portfolio and improve my .NET + React skills.",
                    Metrics: "MVP completed; at least 3 core features implemented; deployed to production; project documented on GitHub.",
                    AchievabilityRationale: "I already have experience with .NET and modern frontend frameworks, and I can dedicate 5–7 hours per week.",
                    Motivation: "Grow as a developer, increase confidence in shipping real products, and potentially showcase it to future employers.",
                    DueDate: new DateTime(2026, 3, 3, 17, 55, 0, DateTimeKind.Utc)));

            CreateSchema<UpdateGoalRequest>()
                .WithSchemaName("UpdateGoalPayload")
                .WithDescription("Model used to update a goal")
                .WithExample(new UpdateGoalRequest(
                    Id: "678a4f92c1a3b05f447e12f9",
                    Title: "Build and publish a full-stack side project",
                    LifeArea: LifeArea.Career,
                    Status: GoalStatus.Active,
                    IsSmartGoal: true,
                    Description: "Create a complete full-stack web application to strengthen my portfolio and improve my .NET + React skills.",
                    Metrics: "MVP completed; 3–5 core features implemented; deployed to production; full documentation on GitHub.",
                    AchievabilityRationale: "I have experience with .NET and React, plus enough weekly time to work on the project.",
                    Motivation: "Improve my practical engineering skills and build a strong portfolio project.",
                    DueDate: new DateTime(2026, 3, 3, 17, 55, 0, DateTimeKind.Utc)));
        }
    }
}
