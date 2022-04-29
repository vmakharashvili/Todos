using Todos.Core.Models;

namespace Todos.Core.Repositories;

public interface IUserRepository
{
    Task<User> GetByName(string username);
    Task<User> Create(User user);
    Task<IEnumerable<User>> GetAll();
}