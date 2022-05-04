using Todos.Core.Models;

namespace Todos.Core.Repositories;

public interface ITodoItemsRepository
{
    Task<TodoItem> Create(TodoItem todoItem);
    Task Update(TodoItem todoItem);
    Task Delete(Guid id);
    Task<IEnumerable<TodoItem>> GetByUserId(int userId);
    Task<TodoItem?> GetById(Guid id);
}