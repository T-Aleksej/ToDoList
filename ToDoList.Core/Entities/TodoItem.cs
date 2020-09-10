using System;
using ToDoList.Core.Entities.Base;

namespace ToDoList.Core.Entities
{
    public class TodoItem : Identity
    {
        /// <summary>
        /// The title of the TodoItem
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content of the TodoItem
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Assigned due date
        /// </summary>
        public DateTime DuetoDateTime { get; set; }

        /// <summary>
        /// Is the task done
        /// </summary>
        public bool Done { get; set; }

        public int TodoListItemId { get; set; }
        public TodoListItem TodoListItem { get; set; }
    }
}