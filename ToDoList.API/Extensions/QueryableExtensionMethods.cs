using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ToDoList.API.ViewModel;

namespace ToDoList.API.Extensions
{
    public static class QueryableExtensionMethods
    {
        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> queryable,
            Expression<Func<T, bool>> selector,
            Func<bool> condition)
        {
            return condition() ? queryable.Where(selector) : queryable;
        }

        public static async Task<PaginatedItemsViewModel<T>> ToPagedListAsync<T>(this IQueryable<T> queryable, int pageIndex, int pageSize)
            where T : class
        {
            var items = queryable
               .Skip(pageSize * (pageIndex - 1))
               .Take(pageSize);

            return new PaginatedItemsViewModel<T>(
                await items.ToListAsync(),
                pageIndex,
                pageSize,
                await items.CountAsync()
                );
        }
    }
}