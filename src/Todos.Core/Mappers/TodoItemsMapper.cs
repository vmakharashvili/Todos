using Todos.Core.Dtos.Todos;
using Todos.Core.Models;

namespace Todos.Core.Mappers;

public static class TodoItemsMapper
{
    public static TodoItemDto ToDto(this TodoItem value) => new TodoItemDto()
    {
        Id = value.Id,
        Description = value.Description,
        IsDone = value.IsDone,
        ParentId = value.ParentId,
        StartTime = value.StartTime,
        EndTime = value.EndTime
    };

    public static IEnumerable<TodoItemDto> ToDtos(this IEnumerable<TodoItem> todoItems)
    {
        return todoItems.Select(x => x.ToDto());
    }
}