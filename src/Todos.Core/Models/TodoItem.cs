namespace Todos.Core.Models;

public class TodoItem
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid? ParentId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual User? User { get; set; }
}