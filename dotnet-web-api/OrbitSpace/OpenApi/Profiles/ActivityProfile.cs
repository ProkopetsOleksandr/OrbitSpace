using OrbitSpace.Application.Dtos.Activity;

namespace OrbitSpace.WebApi.OpenApi.Profiles
{
    public class ActivityProfile : OpenApiSchemaProfile
    {
        public ActivityProfile()
        {
            CreateSchema<ActivityDto>()
                .WithSchemaName("Activity")
                .WithDescription("Represents an Activity")
                .WithExample(new ActivityDto(
                    Id: new Guid("019c577c-c280-7103-ad24-9045db19c9fe"),
                    Name: "Programming",
                    Code: "PRG",
                    CreatedAtUtc: new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc),
                    UpdatedAtUtc: new DateTime(2025, 11, 30, 17, 55, 0, DateTimeKind.Utc)));

            CreateSchema<CreateActivityRequest>()
                .WithSchemaName("CreateActivityPayload")
                .WithDescription("Model used to create an activity")
                .WithExample(new CreateActivityRequest(
                    Name: "Programming",
                    Code: "PRG"));

            CreateSchema<UpdateActivityRequest>()
                .WithSchemaName("UpdateActivityPayload")
                .WithDescription("Model used to update an activity")
                .WithExample(new UpdateActivityRequest(
                    new Guid("019c577c-c280-7103-ad24-9045db19c9fe"),
                    Name: "Programming",
                    Code: "PRG"));
        }
    }
}
