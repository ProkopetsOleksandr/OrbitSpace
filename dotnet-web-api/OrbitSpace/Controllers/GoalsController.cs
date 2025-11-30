using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Dtos.Goal;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.WebApi.Models.Responses;

namespace OrbitSpace.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [Tags("Goals")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public class GoalsController(IGoalService goalService) : ApiControllerBase
    {
        [HttpGet]
        [EndpointSummary("Get all goals")]
        [EndpointDescription("Returns a list of goals associated with the currently authenticated user.")]
        [EndpointName("getAllGoals")]
        [ProducesResponseType<ApiResponse<IEnumerable<GoalDto>>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var data = await goalService.GetAllAsync(CurrentUser.Id);

            return Ok(new ApiResponse<IEnumerable<GoalDto>>(data));
        }
    }
}
