using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ToDoList.Core.Context
{
    public class TodoListContextDesignFactory : IDesignTimeDbContextFactory<TodoListContext>
    {
        public TodoListContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TodoListContext>()
                .UseInMemoryDatabase("TodoList");

            //var optionsBuilder = new DbContextOptionsBuilder<TodoListContext>()
            //    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TodoList;MultipleActiveResultSets=True",
            //    x => x.MigrationsAssembly("ToDoList.Data"));

            return new TodoListContext(optionsBuilder.Options);
        }
    }
}