using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.API.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetQueryable();
        void Add(T model);
        void Remove(T model);
        public void Update(T model);
        Task SaveAsync();
    }
}