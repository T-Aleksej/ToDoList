using ToDoList.Core.Context;
using ToDoList.Core.Model;

namespace ToDoList.API.Repositories
{
    public class TodoListItemRepository : GenericRepository<TodoListItem, TodoListContext>, ITodoListItemRepository
    {
        public TodoListItemRepository(TodoListContext context) : base(context) { }
    }
}