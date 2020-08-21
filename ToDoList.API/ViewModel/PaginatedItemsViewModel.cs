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

        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public long Totalitems { get; private set; }

        public IEnumerable<TEntity> Data { get; private set; }
    }
}