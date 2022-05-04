using Todos.Core.Dtos.Todos;
using Todos.Core.Exceptions;
using Todos.Core.Repositories;
using Todos.Core.Mappers;
using Todos.Core.Models;
using Todos.Core.Services.Abstract;

namespace Todos.Core.Services;

public class TodoService
{
    private readonly ITodoItemsRepository _todoItemsRepository;
    private readonly IAuthService _authService;

    public TodoService(ITodoItemsRepository todoItemsRepository, IAuthService authService)
    {
        _todoItemsRepository = todoItemsRepository;
        _authService = authService;
    }

    public async Task<IEnumerable<TodoItemDto>> GetMyTodoItems()
    {
        var todos = await _todoItemsRepository.GetByUserId(_authService.GetUserId());
        return todos.ToDtos();
    }

    public async Task<TodoItemDto> Create(CreateTodoItemDto? model)
    {
        if(model == null)
        {
            throw new ArgumentException("model is null");
        }
        await ValidateParentIdAndDescription(model);
        ValidateStartAndEndTimes(model);
        
        var todoItem = new TodoItem()
        {
            Description = model.Description,
            IsDone = false,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            ParentId = model.ParentId,
            CreatedAt = DateTime.Now,
            UserId = _authService.GetUserId()
        };
        var todo = await _todoItemsRepository.Create(todoItem);
        return todo.ToDto();
    }

    public async Task Update(UpdateTodoItemDto model)
    {
        await ValidateParentIdAndDescription(model);
        ValidateStartAndEndTimes(model);

        var todoItem = await _todoItemsRepository.GetById(model.Id);
        if (todoItem == null)
        {
            throw new DomainException($"No such TodoItem with id {model.Id}");
        }

        todoItem.Description = model.Description;
        todoItem.StartTime = model.StartTime;
        todoItem.EndTime = model.EndTime;
        todoItem.ParentId = model.ParentId;
        await _todoItemsRepository.Update(todoItem);
    }

    public async Task MakeItDone(Guid id, bool IsDone)
    {
        var todoItem = await _todoItemsRepository.GetById(id);
        if (todoItem == null)
        {
            throw new DomainException($"No such TodoItem with id {id}");
        }

        todoItem.IsDone = IsDone;
        await _todoItemsRepository.Update(todoItem);
    }

    public async Task Remove(Guid id)
    {
        await _todoItemsRepository.Delete(id);
    }

    private async Task ValidateParentIdAndDescription(CreateTodoItemDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Description))
        {
            throw new DomainException("Description required");
        }

        if (model.ParentId.HasValue)
        {
            var parent = await _todoItemsRepository.GetById(model.ParentId.Value);
            if (parent == null)
            {
                throw new DomainException($"No TodoItem with id {model.ParentId.Value}");
            }
        }
    }

    private static void ValidateStartAndEndTimes(CreateTodoItemDto model)
    {
        if (model.StartTime.HasValue)
        {
            if (model.StartTime.Value < DateTime.Now)
            {
                throw new DomainException("Start time can't be in the past");
            }

            if (model.EndTime.HasValue)
            {
                if (model.EndTime.Value < model.StartTime.Value)
                {
                    throw new DomainException("End time can't be prior to start time");
                }
            }
        }

        if (model.EndTime.HasValue && model.EndTime.Value < DateTime.Now)
        {
            throw new DomainException("End time can't be in the past");
        }
    }
}