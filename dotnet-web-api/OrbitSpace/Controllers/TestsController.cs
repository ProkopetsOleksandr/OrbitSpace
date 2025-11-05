using Microsoft.AspNetCore.Mvc;

namespace OrbitSpace.WebApi.Controllers;

[Route("api/[controller]")]
public class TestsController : ApiControllerBase
{
    [HttpGet]
    [Route("exception")]
    public IActionResult GetException()
    {
        throw new Exception("Something went wrong");
    }
    
    [HttpGet]
    [Route("bad-request")]
    public IActionResult GetBadRequest()
    {
        return BadRequest();
    }
}