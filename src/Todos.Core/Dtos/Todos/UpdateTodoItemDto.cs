namespace Todos.Core.Dtos.Todos;

public record UpdateTodoItemDto : CreateTodoItemDto
{
    public Guid Id { get; init; }
}