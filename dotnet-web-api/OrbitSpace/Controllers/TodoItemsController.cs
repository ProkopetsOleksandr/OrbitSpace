using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitSpace.Application.Interfaces.Services;
using OrbitSpace.Application.Models.Dtos.Todo;
using OrbitSpace.WebApi.Models.Responses.TodoItems;

namespace OrbitSpace.WebApi.Controllers;

[Authorize]
[Route("api/todo-items")]
[Tags("Todo Items")]
public class TodoItemsController(ITodoItemService todoItemService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<GetTodoItemsResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var data = await todoItemService.GetAllAsync(ApplicationUser.Id);

        var responseData = data.Select(m =>
            new TodoItemResource(m.Id, m.Title, m.CreatedAt, m.UpdatedAt, m.Status, m.StatusDescription));

        return Ok(new GetTodoItemsResponse(responseData));
    }

    [HttpGet("{id}")]
    [ProducesResponseType<TodoItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await todoItemService.GetByIdAsync(id, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return Problem();
        }

        var data = result.Data;
        var responseData = new TodoItemResource(data.Id, data.Title, data.CreatedAt, data.UpdatedAt, data.Status, data.StatusDescription);

        return Ok(new TodoItemResponse(responseData));
    }

    [HttpPost]
    [ProducesResponseType<TodoItemResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemRequest request)
    {
        var createTodoItemRequestDto = new CreateTodoItemDto(request.Title);

        var result = await todoItemService.CreateAsync(createTodoItemRequestDto, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return Problem();
        }

        var data = result.Data;
        var responseData = new TodoItemResource(data.Id, data.Title, data.CreatedAt, data.UpdatedAt, data.Status, data.StatusDescription);

        return CreatedAtAction(nameof(GetById), new { data.Id }, new TodoItemResponse(responseData));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] TodoItemResource model)
    {
        if (model.Id != id)
        {
           return Problem();
        }

        var todoItemDto = new TodoItemDto(model.Id, model.Title, model.CreatedAt, model.UpdatedAt, model.Status, model.StatusDescription);

        var result = await todoItemService.UpdateAsync(todoItemDto, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return Problem();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await todoItemService.DeleteAsync(id, ApplicationUser.Id);
        if (!result.IsSuccess)
        {
            return Problem();
        }

        return NoContent();
    }
}