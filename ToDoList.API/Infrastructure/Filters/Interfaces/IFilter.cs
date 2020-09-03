using System.Linq;

namespace TToDoList.API.Infrastructure.Filters.Interfaces
{
    public interface IFilter<T, U>
    {
        IQueryable<T> Filtration(IQueryable<T> queryable, U parameters);
    }
}