using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.Application.Services;

namespace OrbitSpace.WebApi.Controllers;

[Route("api/[controller]")]
public class TodoItemController(TodoItemService todoItemService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string userId)
    {
        return Ok(await todoItemService.GetAllAsync(userId));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var result = await todoItemService.GetByIdAsync(id);
        
        return !result.IsSuccess ? HandleFailure(result.Error) : Ok(result.Data);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTodoItemDto model)
    {
        var result = await todoItemService.CreateAsync(model);

        return CreatedAtAction("", new { id = result.Data!.Id }, result.Data);
    }
}