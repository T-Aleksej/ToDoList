using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToDoList.Core.Entities.Base;

namespace ToDoList.Core.Entities
{
    public class TodoListItem : Identity
    {
        /// <summary>
        /// The title of the TodoListItem
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the TodoListItem
        /// </summary>
        public string Description { get; set; }

        public List<TodoItem> TodoItems { get; set; }
    }
}