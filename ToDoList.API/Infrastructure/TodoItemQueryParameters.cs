using System;

namespace ToDoList.API.Infrastructure
{
    public class TodoItemQueryParameters : QueryParameters
    {
        public int TodoListItem { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsComplete { get; set; }
    }
}