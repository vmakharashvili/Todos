using Microsoft.EntityFrameworkCore;
using Todos.Core.Models;
using Todos.Core.Repositories;

namespace Todos.Data.Repositories;

public class TodoItemsRepository : ITodoItemsRepository
{
    private readonly TodosDbContext _context;

    public TodoItemsRepository(TodosDbContext context)
    {
        _context = context;
    }

    public async Task<TodoItem> Create(TodoItem todoItem)
    {
        _context.Todos.Add(todoItem);
        await _context.SaveChangesAsync();
        return todoItem;
    }

    public async Task Update(TodoItem todoItem)
    {
        _context.Todos.Update(todoItem);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var todoItem = await _context.Todos.FirstOrDefaultAsync(x => x.Id == id);
        if (todoItem != null)
        {
            _context.Todos.Remove(todoItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TodoItem>> GetByUserId(int userId)
    {
        return await _context.Todos.Where(x => x.UserId == userId).ToListAsync();
    }

    public Task<TodoItem?> GetById(Guid id)
    {
        return _context.Todos.FirstOrDefaultAsync(x => x.Id == id);
    }
}