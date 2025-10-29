using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.Application.Services;

namespace OrbitSpace.WebApi.Controllers;

[Route("api/[controller]")]
public class TodoItemController(TodoItemService todoItemService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = "test";

        return Ok(await todoItemService.GetAllAsync(userId));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await todoItemService.GetByIdAsync(id);
        
        return !result.IsSuccess ? HandleFailure(result.Error) : Ok(result.Data);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemDto model)
    {
        var result = await todoItemService.CreateAsync(model);
        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] TodoItemDto model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var result = await todoItemService.UpdateAsync(model);
        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await todoItemService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }
}