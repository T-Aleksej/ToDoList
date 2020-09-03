using System;
using System.Collections.Generic;

namespace ToDoList.API.ViewModel
{
    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    {
        public PaginatedItemsViewModel(IEnumerable<TEntity> data, int pageIndex, int pageSize, int totalCount)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
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
        public int TotalCount { get; private set; }

        /// <summary>
        /// Total pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// The data playload
        /// </summary>
        public IEnumerable<TEntity> Data { get; private set; }
    }
}