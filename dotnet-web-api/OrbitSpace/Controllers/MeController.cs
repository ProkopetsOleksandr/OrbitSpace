using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.WebApi.Models.Responses;

namespace OrbitSpace.WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[Tags("Me")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
public class MeController : ApiControllerBase
{
    private class UserDtoMock
    {
        public Guid Id {get; set; }
    }
    
    [HttpGet]
    [EndpointSummary("Get user info")]
    [EndpointDescription("Returns info of currently authenticated user")]
    [EndpointName("getMe")]
    [ProducesResponseType<ApiResponse<UserDtoMock>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public IActionResult Me()
    {
        return Ok(new ApiResponse<UserDtoMock>(new UserDtoMock() { Id = CurrentUser.Id }));
    }
}