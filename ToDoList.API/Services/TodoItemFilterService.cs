using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Extensions;
using ToDoList.API.Infrastructure;
using ToDoList.Core.Model;

namespace ToDoList.API.Services
{
    public class TodoItemFilterService : ITodoItemFilterService
    {
        public async Task<(IQueryable<T>, long)> FilteredByNameAndDateAndComplete<T>(IQueryable<T> todoItems, TodoItemQueryParameters queryParam) where T : TodoItem
        {
            todoItems = todoItems.WhereIf(
                x => x.Title.Trim().Contains(queryParam.Title.Trim()),
                () => !string.IsNullOrWhiteSpace(queryParam.Title));

            todoItems = todoItems.WhereIf(
                x => x.Done == queryParam.IsComplete.Value,
                () => queryParam.IsComplete.HasValue);

            todoItems = todoItems.WhereIf(
                x => x.DuetoDateTime.Date == queryParam.Date.Value.Date,
                () => queryParam.Date.HasValue);

            var totalitems = await todoItems.LongCountAsync();

            var page = todoItems.Pagination(queryParam);

            return (page, totalitems);
        }
    }
}