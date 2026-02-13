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
        private const string GoalNotFoundMessageTemplate = "Goal with id {0} not found";
        
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
        
        [HttpGet("{id}")]
        [EndpointSummary("Get goal details")]
        [EndpointDescription("Returns goal details with specified Id associated with the currently authenticated user.")]
        [EndpointName("getGoalDetailsById")]
        [ProducesResponseType<ApiResponse<GoalDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var goal = await goalService.GetGoalDetailsAsync(id, CurrentUser.Id);
            if (goal == null)
            {
                return NotFoundProblem(string.Format(GoalNotFoundMessageTemplate, id));
            }

            return Ok(new ApiResponse<GoalDto>(goal));
        }
        
        [HttpPost]
        [EndpointSummary("Create goal")]
        [EndpointName("createGoal")]
        [ProducesResponseType<ApiResponse<GoalDto>>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateGoalRequest request)
        {
            var result = await goalService.CreateAsync(request, CurrentUser.Id);
            if (!result.IsSuccess)
            {
                return HandleError(result.Error);
            }

            return CreatedAtAction(nameof(GetById), new { result.Data.Id }, new ApiResponse<GoalDto>(result.Data));
        }
        
        [HttpPut("{id}")]
        [EndpointSummary("Update goal")]
        [EndpointName("updateGoal")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGoalRequest request)
        {
            if (request.Id != id)
            {
                return BadRequestProblem("Invalid id");
            }

            var result = await goalService.UpdateAsync(request, CurrentUser.Id);
            if (!result.IsSuccess)
            {
                return HandleError(result.Error);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [EndpointSummary("Delete goal")]
        [EndpointName("deleteGoal")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await goalService.DeleteAsync(id, CurrentUser.Id))
            {
                return NotFoundProblem(string.Format(GoalNotFoundMessageTemplate, id));
            }

            return NoContent();
        }
    }
}
