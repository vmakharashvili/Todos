namespace Todos.Core.Dtos.Todos;

public class TodoItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid? ParentId { get; set; }
}