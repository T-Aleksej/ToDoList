using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Infrastructure;
using ToDoList.Core.Model;

namespace ToDoList.API.Services.Interfaces
{
    public interface ITodoItemFilterService
    {
        Task<(IQueryable<T>, long)> FilteredByNameAndDateAndComplete<T>(IQueryable<T> todoItems, TodoItemQueryParameters queryParam) where T : TodoItem;
    }
}