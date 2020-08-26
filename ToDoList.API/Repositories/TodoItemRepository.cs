using ToDoList.Core.Context;
using ToDoList.Core.Model;

namespace ToDoList.API.Repositories
{
    public class TodoItemRepository : GenericRepository<TodoItem, TodoListContext>, ITodoItemRepository
    {
        public TodoItemRepository(TodoListContext context) : base(context) { }
    }
}