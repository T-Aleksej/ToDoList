using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.API.Infrastructure
{
    public class TodoItemQueryParameters : QueryParameters
    {
        /// <summary>
        /// TodoListItem ID
        /// </summary>
        [Required] public int TodoListItem { get; set; }

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