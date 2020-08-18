using System.Collections.Generic;

namespace ToDoList.Core.Model
{
    public class TodoListItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<TodoItem> TodoItems { get; set; }
    }
}