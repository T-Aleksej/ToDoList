using ToDoList.Core.Entities;
using TToDoList.API.Infrastructure.Filters.Interfaces;

namespace ToDoList.API.Infrastructure.Filters.Interfaces
{
    public interface IFilterWrapper
    {
        IFilter<TodoItem, TodoItemQueryParameters> TodoItemFilter { get; }
        IFilter<TodoListItem, TodoListItemQueryParameters> TodoListItemFilter { get; }
    }
}