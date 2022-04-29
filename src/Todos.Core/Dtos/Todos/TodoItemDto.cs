namespace Todos.Core.Dtos.Todos;

public record TodoItemDto
{
    public Guid Id { get; init; }
    public string Description { get; init; }
    public bool IsDone { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public Guid? ParentId { get; init; }
}