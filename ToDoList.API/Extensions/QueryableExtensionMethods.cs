using System;
using System.Linq;
using System.Linq.Expressions;

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
    }
}