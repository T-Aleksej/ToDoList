using Microsoft.EntityFrameworkCore;
using ToDoList.Core.Model;

namespace ToDoList.Core.Context
{
    public class TodoListContext : DbContext
    {
        public TodoListContext(DbContextOptions<TodoListContext> options) : base(options) { }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TodoListItem> TodoListItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(TodoListContext).Assembly);
        }
    }
}