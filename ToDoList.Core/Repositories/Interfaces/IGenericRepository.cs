using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Core.Repositories.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetQueryable();
        void Add(T model);
        void Remove(T model);
        void Update(T model);
        Task SaveAsync();
    }
}