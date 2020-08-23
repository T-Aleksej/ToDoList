using System.Collections.Generic;

namespace ToDoList.API.ViewModel
{
    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    {
        public PaginatedItemsViewModel(int pageIndex, int pageSize, long totalitems, IEnumerable<TEntity> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Totalitems = totalitems;
            Data = data;
        }

        /// <summary>
        /// Page number
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Total items
        /// </summary>
        public long Totalitems { get; private set; }

        /// <summary>
        /// The data playload
        /// </summary>
        public IEnumerable<TEntity> Data { get; private set; }
    }
}