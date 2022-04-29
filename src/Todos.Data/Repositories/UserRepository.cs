using Microsoft.EntityFrameworkCore;
using Todos.Core.Models;
using Todos.Core.Repositories;

namespace Todos.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TodosDbContext _context;

    public UserRepository(TodosDbContext context)
    {
        _context = context;
    }

    public Task<User> GetByName(string username)
    {
        return _context.Users.Where(x => x.Username == username).FirstOrDefaultAsync();
    }

    public async Task<User> Create(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }
}