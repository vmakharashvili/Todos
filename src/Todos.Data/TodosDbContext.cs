using Microsoft.EntityFrameworkCore;
using Todos.Core.Models;

namespace Todos.Data;

public class TodosDbContext : DbContext
{
    public TodosDbContext(DbContextOptions<TodosDbContext> options):base(options)
    {
        
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>().HasOne(x => x.User).WithMany(x => x.TodoItems)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<User>().HasData(new User()
        {
            Id = 1,
            Username = "Admin",
            PasswordHash = "6f40424be4e4f61c281d1928c9e6416774d6b96522b2f869d324669391adf59e",
            IsActive = true
        });
    }
}