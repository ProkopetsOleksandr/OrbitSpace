using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Dtos.TodoItem;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.WebApi.Models.Responses.TodoItems;

namespace OrbitSpace.WebApi.Controllers;

[Authorize]
[Route("api/todo-items")]
[Tags("Todo Items")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
public class TodoItemsController(ITodoItemService todoItemService) : ApiControllerBase
{
    private const string TodoItemNotFoundMessageTemplate = "Todo-item with id:{0} not found";

    [HttpGet]
    [EndpointSummary("Get all todo items")]
    [EndpointDescription("Returns a list of todo items associated with the currently authenticated user.")]
    [ProducesResponseType<GetTodoItemsResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var data = await todoItemService.GetAllAsync(CurrentUser.Id);

        return Ok(new GetTodoItemsResponse(data));
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get todo item")]
    [EndpointDescription("Returns todo item with specified Id associated with the currently authenticated user.")]
    [ProducesResponseType<TodoItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var todoItem = await todoItemService.GetByIdAsync(id, CurrentUser.Id);
        if (todoItem == null)
        {
            return NotFoundProblem(string.Format(TodoItemNotFoundMessageTemplate, id));
        }

        return Ok(new TodoItemResponse(todoItem));
    }

    [HttpPost]
    [EndpointSummary("Create todo item")]
    [ProducesResponseType<TodoItemResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [Description("Create todo item payload")]
        [FromBody] CreateTodoItemDto request)
    {
        var result = await todoItemService.CreateAsync(request, CurrentUser.Id);
        if (!result.IsSuccess)
        {
            return HandleError(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { result.Data.Id }, new TodoItemResponse(result.Data));
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update todo item")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTodoItemDto request)
    {
        if (request.Id != id)
        {
            return BadRequestProblem("Invalid id");
        }

        var result = await todoItemService.UpdateAsync(request, CurrentUser.Id);
        if (!result.IsSuccess)
        {
            return HandleError(result.Error);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Delete todo item")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        if (!await todoItemService.DeleteAsync(id, CurrentUser.Id))
        {
            return NotFoundProblem(string.Format(TodoItemNotFoundMessageTemplate, id));
        }

        return NoContent();
    }
}