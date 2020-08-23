using System;

namespace ToDoList.API.Infrastructure
{
    public class QueryParameters
    {
        const int _maxPageSize = 100;
        private int _pageSize;

        /// <summary>
        /// Page number
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize
        {
            get => Math.Max(_pageSize, 10);
            set => _pageSize = Math.Min(_maxPageSize, value);
        }
    }
}