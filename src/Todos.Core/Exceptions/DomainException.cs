namespace Todos.Core.Exceptions;

public class DomainException : ArgumentException
{
    public DomainException(string message):base(message)
    {
        
    }
}