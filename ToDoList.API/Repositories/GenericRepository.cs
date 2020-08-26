using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.API.Repositories
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        protected GenericRepository(TContext context)
        {
            _context = context;
        }

        protected readonly TContext _context;

        public void Add(TEntity model) => _context.Set<TEntity>().Add(model);

        public IQueryable<TEntity> GetQueryable() => _context.Set<TEntity>().AsQueryable();

        public async Task<TEntity> GetByIdAsync(int id) => await _context.Set<TEntity>().FindAsync(id);

        public void Remove(TEntity model) => _context.Set<TEntity>().Remove(model);

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public void Update(TEntity model) => _context.Entry<TEntity>(model).State = EntityState.Modified;
    }
}