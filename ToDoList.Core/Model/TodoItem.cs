using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Core.Model
{
    public class TodoItem
    {
        public int Id { get; set; }

        /// <summary>
        /// The title of the TodoItem
        /// </summary>
        [Required] [MaxLength(50)] public string Title { get; set; }

        /// <summary>
        /// The content of the TodoItem
        /// </summary>
        [Required] [MaxLength(1000)] public string Content { get; set; }

        /// <summary>
        /// Assigned due date
        /// </summary>
        [Required] public DateTime DuetoDateTime { get; set; }

        /// <summary>
        /// Is the task done
        /// </summary>
        [Required] public bool Done { get; set; }

        public int TodoListItemId { get; set; }
        public TodoListItem TodoListItem { get; set; }
    }
}