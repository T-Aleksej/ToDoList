using System;

namespace ToDoList.API.Infrastructure
{
    public class TodoItemQueryParameters : QueryParameters
    {
        public int TodoListItemId { get; set; }
        public string Title { get; set; }
        public DateTime? DateOfCompletion { get; set; }
        public bool? IsComplete { get; set; }
    }
}