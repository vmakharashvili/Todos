namespace Todos.Core.Dtos.Users;

public record UserDto
{
    public int Id { get; init; }
    public string Username { get; init; }
    public bool IsActive { get; init; }
}