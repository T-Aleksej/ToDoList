using System;
using System.Linq;
using System.Linq.Expressions;
using ToDoList.API.Infrastructure;

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

        public static IQueryable<T> Pagination<T>(this IQueryable<T> todoItems, QueryParameters queryParam)
        {
            todoItems = todoItems
               .Skip(queryParam.PageSize * (queryParam.PageIndex - 1))
               .Take(queryParam.PageSize);

            return todoItems;
        }
    }
}