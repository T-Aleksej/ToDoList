using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Infrastructure;
using ToDoList.Core.Model;

namespace ToDoList.API.Services.Interfaces
{
    public interface ITodoListItemFilterService
    {
        Task<(IQueryable<T>, long)> FilteredByName<T>(IQueryable<T> todoListItems, TodoListItemQueryParameters queryParam) where T : TodoListItem;
    }
}