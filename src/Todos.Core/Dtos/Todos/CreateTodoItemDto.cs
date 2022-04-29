namespace Todos.Core.Dtos.Todos;

public class CreateTodoItemDto
{
    public string Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid? ParentId { get; set; }
}