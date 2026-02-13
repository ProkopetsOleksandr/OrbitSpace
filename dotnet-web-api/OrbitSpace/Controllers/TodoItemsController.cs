using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Dtos.TodoItem;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.WebApi.Models.Responses;

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
    [EndpointName("getAllTodoItems")]
    [ProducesResponseType<ApiResponse<IEnumerable<TodoItemDto>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var data = await todoItemService.GetAllAsync(CurrentUser.Id);

        return Ok(new ApiResponse<IEnumerable<TodoItemDto>>(data));
    }

    [HttpGet("{id}")]
    [EndpointSummary("Get todo item")]
    [EndpointDescription("Returns todo item with specified Id associated with the currently authenticated user.")]
    [EndpointName("getTodoItemById")]
    [ProducesResponseType<ApiResponse<TodoItemDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var todoItem = await todoItemService.GetByIdAsync(id, CurrentUser.Id);
        if (todoItem == null)
        {
            return NotFoundProblem(string.Format(TodoItemNotFoundMessageTemplate, id));
        }

        return Ok(new ApiResponse<TodoItemDto>(todoItem));
    }

    [HttpPost]
    [EndpointSummary("Create todo item")]
    [EndpointName("createTodoItem")]
    [ProducesResponseType<ApiResponse<TodoItemDto>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemDto request)
    {
        var result = await todoItemService.CreateAsync(request, CurrentUser.Id);
        if (!result.IsSuccess)
        {
            return HandleError(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { result.Data.Id }, new ApiResponse<TodoItemDto>(result.Data));
    }

    [HttpPut("{id}")]
    [EndpointSummary("Update todo item")]
    [EndpointName("updateTodoItem")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoItemDto request)
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
    [EndpointName("deleteTodoItem")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await todoItemService.DeleteAsync(id, CurrentUser.Id))
        {
            return NotFoundProblem(string.Format(TodoItemNotFoundMessageTemplate, id));
        }

        return NoContent();
    }
}