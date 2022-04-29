namespace Todos.Core.Dtos.Todos;

public record CreateTodoItemDto
{
    public string Description { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public Guid? ParentId { get; init; }
}