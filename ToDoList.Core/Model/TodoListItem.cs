using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Core.Model
{
    public class TodoListItem
    {
        public int Id { get; set; }

        /// <summary>
        /// The title of the TodoListItem
        /// </summary>
        [Required] [MaxLength(50)] public string Title { get; set; }

        /// <summary>
        /// The description of the TodoListItem
        /// </summary>
        [MaxLength(100)] public string Description { get; set; }

        public List<TodoItem> TodoItems { get; set; }
    }
}