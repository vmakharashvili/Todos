namespace Todos.Core.Dtos.Users;

public record CreateUserDto
{
    public string? Username { get; init; }
    public string? Password { get; init; }
}