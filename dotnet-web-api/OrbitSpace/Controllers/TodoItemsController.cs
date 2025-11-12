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
    [ProducesResponseType<GetTodoItemsResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var data = await todoItemService.GetAllAsync(CurrentUser.Id);

        var responseData = data.Select(m =>
            new TodoItemResource(m.Id, m.Title, m.CreatedAt, m.UpdatedAt, m.Status, m.StatusDescription));

        return Ok(new GetTodoItemsResponse(responseData));
    }

    [HttpGet("{id}")]
    [ProducesResponseType<TodoItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var todoItem = await todoItemService.GetByIdAsync(id, CurrentUser.Id);
        if (todoItem == null)
        {
            return NotFoundProblem(string.Format(TodoItemNotFoundMessageTemplate, id));
        }
        
        var responseData = new TodoItemResource(todoItem.Id, todoItem.Title, todoItem.CreatedAt, todoItem.UpdatedAt, todoItem.Status, todoItem.StatusDescription);

        return Ok(new TodoItemResponse(responseData));
    }

    [HttpPost]
    [ProducesResponseType<TodoItemResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoItemRequest request)
    {
        var createTodoItemRequestDto = new CreateTodoItemDto(request.Title);

        var result = await todoItemService.CreateAsync(createTodoItemRequestDto, CurrentUser.Id);
        if (!result.IsSuccess)
        {
            return HandleError(result.Error);
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
            return BadRequestProblem("Invalid id");
        }

        var todoItemDto = new TodoItemDto(model.Id, model.Title, model.CreatedAt, model.UpdatedAt, model.Status, model.StatusDescription);

        var result = await todoItemService.UpdateAsync(todoItemDto, CurrentUser.Id);
        if (!result.IsSuccess)
        {
            return HandleError(result.Error);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
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