using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Dtos.Activity;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.WebApi.Models.Responses;

namespace OrbitSpace.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [Tags("Activities")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public class ActivitiesController(IActivityService activityService) : ApiControllerBase
    {
        private const string ActivityNotFoundMessageTemplate = "Activity with id {0} not found";

        [HttpGet]
        [EndpointSummary("Get all activities")]
        [EndpointDescription("Returns a list of activities associated with the currently authenticated user.")]
        [EndpointName("getAllActivities")]
        [ProducesResponseType<ApiResponse<IEnumerable<ActivityDto>>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var data = await activityService.GetAllAsync(CurrentUser.Id);

            return Ok(new ApiResponse<IEnumerable<ActivityDto>>(data));
        }

        [HttpGet("{id}")]
        [EndpointSummary("Get activity by id")]
        [EndpointDescription("Returns activity details with specified Id associated with the currently authenticated user.")]
        [EndpointName("getActivityById")]
        [ProducesResponseType<ApiResponse<ActivityDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var activity = await activityService.GetByIdAsync(id, CurrentUser.Id);
            if (activity == null)
            {
                return NotFoundProblem(string.Format(ActivityNotFoundMessageTemplate, id));
            }

            return Ok(new ApiResponse<ActivityDto>(activity));
        }

        [HttpPost]
        [EndpointSummary("Create activity")]
        [EndpointName("createActivity")]
        [ProducesResponseType<ApiResponse<ActivityDto>>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateActivityRequest request)
        {
            var result = await activityService.CreateAsync(request, CurrentUser.Id);
            if (!result.IsSuccess)
            {
                return HandleError(result.Error);
            }

            return CreatedAtAction(nameof(GetById), new { result.Data.Id }, new ApiResponse<ActivityDto>(result.Data));
        }

        [HttpPut("{id}")]
        [EndpointSummary("Update activity")]
        [EndpointName("updateActivity")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateActivityRequest request)
        {
            if (request.Id != id)
            {
                return BadRequestProblem("Invalid id");
            }

            var result = await activityService.UpdateAsync(request, CurrentUser.Id);
            if (!result.IsSuccess)
            {
                return HandleError(result.Error);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [EndpointSummary("Delete activity")]
        [EndpointName("deleteActivity")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await activityService.DeleteAsync(id, CurrentUser.Id))
            {
                return NotFoundProblem(string.Format(ActivityNotFoundMessageTemplate, id));
            }

            return NoContent();
        }
    }
}
