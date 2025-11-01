using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.WebApi.Models.Responses;
using OrbitSpace.WebApi.Models.Responses.Base;

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
    [ProducesResponseType(typeof(TodoItemsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var data = await _todoItemService.GetAllAsync(ApplicationUser.Id);

        return new OkObjectResult(new TodoItemsResponse(data));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _todoItemService.GetByIdAsync(id, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return ApiErrorResponse(result.Error);
        }

        return new OkObjectResult(new TodoItemResponse(result.Data));
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemDto model)
    {
        var result = await _todoItemService.CreateAsync(model, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return ApiErrorResponse(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { result.Data.Id }, new TodoItemResponse(result.Data));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] TodoItemDto model)
    {
        var result = await _todoItemService.UpdateAsync(model, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return ApiErrorResponse(result.Error);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _todoItemService.DeleteAsync(id, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return ApiErrorResponse(result.Error);
        }

        return NoContent();
    }
}