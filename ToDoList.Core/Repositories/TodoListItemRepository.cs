using ToDoList.Core.Context;
using ToDoList.Core.Model;
using ToDoList.Core.Repositories.Interfaces;

namespace ToDoList.Core.Repositories
{
    public class TodoListItemRepository : GenericRepository<TodoListItem, TodoListContext>, ITodoListItemRepository
    {
        public TodoListItemRepository(TodoListContext context) : base(context) { }
    }
}