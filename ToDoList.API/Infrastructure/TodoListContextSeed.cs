using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoList.Core.Model;

namespace ToDoList.API.Infrastructure
{
    public class TodoListContextSeed
    {
        public async Task SeedAsync(Core.Context.TodoListContext context)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.TodoListItems.AddRangeAsync(GetPreconfiguredTodoListItems());
            await context.SaveChangesAsync();

            await context.TodoItems.AddRangeAsync(GetPreconfiguredTodoItems());
            await context.SaveChangesAsync();
        }

        private IEnumerable<TodoListItem> GetPreconfiguredTodoListItems()
        {
            return new List<TodoListItem>()
            {
                new TodoListItem() {Title = "Работа", Description = "Рабочие дела" },
                new TodoListItem() {Title = "Личное", Description = "Домашние дела" },
            };
        }

        private IEnumerable<TodoItem> GetPreconfiguredTodoItems()
        {
            return new List<TodoItem>()
            {
                new TodoItem() { TodoListItemId = 1, Title = "ToDoList", Content = "Закончить проект", Done = true, DuetoDateTime = DateTime.Now },
                new TodoItem() { TodoListItemId = 2, Title = "Животные", Content = "Накормить кота", Done = false, DuetoDateTime = DateTime.Now },
                new TodoItem() { TodoListItemId = 2, Title = "Уборка", Content = "Прибраться на кухне", Done = true, DuetoDateTime = DateTime.Now }
            };
        }
    }
}