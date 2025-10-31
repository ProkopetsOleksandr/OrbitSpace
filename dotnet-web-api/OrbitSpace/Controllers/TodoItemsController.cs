using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.WebApi.Models;

namespace OrbitSpace.WebApi.Controllers;

[Authorize]
[Route("api/todo-items")]
[Tags("Todo Items")]
public class TodoItemsController : ApiControllerBase
{
    private readonly ITodoItemService _todoItemService;

    public TodoItemsController(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TodoItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var data = await _todoItemService.GetAllAsync(ApplicationUser.Id);

        return Ok(GetApiResponse(data));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TodoItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _todoItemService.GetByIdAsync(id, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(GetApiResponse(new List<TodoItemDto> { result.Data }));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TodoItemDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemDto model)
    {
        var result = await _todoItemService.CreateAsync(model, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, GetApiResponse(result.Data));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] TodoItemDto model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var result = await _todoItemService.UpdateAsync(model, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _todoItemService.DeleteAsync(id, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }
}