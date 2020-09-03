using System;

namespace ToDoList.API.Infrastructure
{
    public class TodoItemQueryParameters
    {
        /// <summary>
        /// TodoItem title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// TodoItem assigned due date
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Is the task done
        /// </summary>
        public bool? IsComplete { get; set; }
    }
}