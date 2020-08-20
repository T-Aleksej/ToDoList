using System;

namespace ToDoList.API.Infrastructure
{
    public class QueryParameters
    {
        const int _maxPageSize = 100;
        private int _pageSize;

        public int PageIndex { get; set; } = 1;
        public int PageSize
        {
            get => Math.Max(_pageSize, 1);
            set => _pageSize = Math.Min(_maxPageSize, value);
        }
    }
}