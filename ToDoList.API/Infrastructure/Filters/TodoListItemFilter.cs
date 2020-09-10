using System.Linq;
using ToDoList.API.Extensions;
using ToDoList.Core.Entities;
using TToDoList.API.Infrastructure.Filters.Interfaces;

namespace ToDoList.API.Infrastructure.Filters
{
    public class TodoListItemFilter : IFilter<TodoListItem, TodoListItemQueryParameters>
    {
        public IQueryable<TodoListItem> Filtration(IQueryable<TodoListItem> queryable, TodoListItemQueryParameters param)
        {
            queryable = queryable.WhereIf(
                x => x.Title.Trim().Contains(param.Title.Trim()),
                () => !string.IsNullOrWhiteSpace(param.Title));

            return queryable;
        }
    }
}