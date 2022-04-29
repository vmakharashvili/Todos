namespace Todos.Core.Dtos.Todos;

public class UpdateTodoItemDto : CreateTodoItemDto
{
    public Guid Id { get; set; }
}