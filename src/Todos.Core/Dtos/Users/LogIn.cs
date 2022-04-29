namespace Todos.Core.Dtos.Users;

public record LogInDto
{
    public string Username { get; init; }
    public string Password { get; init; }
}