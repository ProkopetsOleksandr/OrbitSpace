using Microsoft.AspNetCore.Mvc;

namespace OrbitSpace.WebApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/[controller]")]
public class TestsController : ApiControllerBase
{
    [HttpGet]
    [Route("exception")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public IActionResult GetException()
    {
        throw new Exception("Something went wrong");
    }
    
    [HttpGet]
    [Route("bad-request")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public IActionResult GetBadRequest()
    {
        return BadRequest();
    }
}