using ToDoList.API.Infrastructure.Filters.Interfaces;
using ToDoList.Core.Model;
using TToDoList.API.Infrastructure.Filters.Interfaces;

namespace ToDoList.API.Infrastructure.Filters
{
    public class FilterWrapper : IFilterWrapper
    {
        public IFilter<TodoItem, TodoItemQueryParameters> TodoItemFilter => new TodoItemFilter();
        public IFilter<TodoListItem, TodoListItemQueryParameters> TodoListItemFilter => new TodoListItemFilter();
    }
}