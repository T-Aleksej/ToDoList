using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Extensions;
using ToDoList.API.Infrastructure;
using ToDoList.Core.Model;

namespace ToDoList.API.Services
{
    public class TodoListItemFilterService : ITodoListItemFilterService
    {
        public async Task<(IQueryable<T>, long)> FilteredByName<T>(IQueryable<T> todoListItems, TodoListItemQueryParameters queryParam) where T : TodoListItem
        {
            todoListItems = todoListItems.WhereIf(
                x => x.Title.Trim().Contains(queryParam.Title.Trim()),
                () => !string.IsNullOrWhiteSpace(queryParam.Title));

            var totalitems = await todoListItems.LongCountAsync();

            var page = todoListItems.Pagination(queryParam);

            return (page, totalitems);
        }
    }
}