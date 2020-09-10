using ToDoList.Core.Context;
using ToDoList.Core.Entities;
using ToDoList.Core.Repositories.Interfaces;

namespace ToDoList.Core.Repositories
{
    public class TodoItemRepository : GenericRepository<TodoItem, TodoListContext>, ITodoItemRepository
    {
        public TodoItemRepository(TodoListContext context) : base(context) { }
    }
}