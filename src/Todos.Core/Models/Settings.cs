namespace Todos.Core.Models;

public class Settings
{
    public string SecretKey { get; set; }
    public Jwt Jwt { get; set; }
}

public class Jwt
{
    public string Secret { get; set; }
}