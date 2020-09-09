using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ToDoList.Core.Entities.Base;

namespace ToDoList.Core.Entities
{
    public class TodoItem : Identity
    {
        //public int Id { get; set; }

        /// <summary>
        /// The title of the TodoItem
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(50, ErrorMessage = "Title can't be longer than 50 characters")] 
        public string Title { get; set; }

        /// <summary>
        /// The content of the TodoItem
        /// </summary>
        [Required(ErrorMessage = "Content is required")]
        [MaxLength(1000, ErrorMessage = "Content can't be longer than 1000 characters")] 
        public string Content { get; set; }

        /// <summary>
        /// Assigned due date
        /// </summary>
        [Required(ErrorMessage = "DuetoDateTime is required")]
        [DataType(DataType.Date)] 
        public DateTime DuetoDateTime { get; set; }

        /// <summary>
        /// Is the task done
        /// </summary>
        [Required(ErrorMessage = "Done is required")]
        [DefaultValue(false)] 
        public bool Done { get; set; }

        public int TodoListItemId { get; set; }
        public TodoListItem TodoListItem { get; set; }
    }
}