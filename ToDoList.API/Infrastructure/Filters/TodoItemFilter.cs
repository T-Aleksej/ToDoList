using System.Linq;
using ToDoList.API.Extensions;
using ToDoList.Core.Entities;
using TToDoList.API.Infrastructure.Filters.Interfaces;

namespace ToDoList.API.Infrastructure.Filters
{
    public class TodoItemFilter : IFilter<TodoItem, TodoItemQueryParameters>
    {
        public IQueryable<TodoItem> Filtration(IQueryable<TodoItem> queryable, TodoItemQueryParameters param)
        {
            queryable = queryable.WhereIf(
                x => x.Title.Trim().Contains(param.Title.Trim()),
                () => !string.IsNullOrWhiteSpace(param.Title));

            queryable = queryable.WhereIf(
                x => x.Done == param.IsComplete.Value,
                () => param.IsComplete.HasValue);

            queryable = queryable.WhereIf(
                x => x.DuetoDateTime.Date == param.Date.Value.Date,
                () => param.Date.HasValue);

            return queryable;
        }
    }
}