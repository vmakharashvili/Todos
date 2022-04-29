using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todos.API.Models;
using Todos.Core.Dtos.Todos;
using Todos.Core.Services;

namespace Todos.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class TodoItemsController : Controller
{
    private readonly TodoService _todoService;
    // GET
    public TodoItemsController(TodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<IEnumerable<TodoItemDto>> Index()
    {
        return await _todoService.GetMyTodoItems();
    }

    [HttpPost]
    public async Task<TodoItemDto> Create([FromBody] CreateTodoItemDto model)
    {
        return await _todoService.Create(model);
    }

    [HttpPut]
    public async Task Update([FromBody] UpdateTodoItemDto model)
    {
        await _todoService.Update(model);
    }

    [HttpPut("Done")]
    public async Task MarkIsDone([FromBody] MarkIsDoneModel model)
    {
        await _todoService.MakeItDone(model.Id, model.IsDone);
    }

    [HttpDelete("{id}")]
    public async Task Remove(Guid id) => await _todoService.Remove(id);
}