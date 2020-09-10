using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ToDoList.Core.Entities.Base;

namespace ToDoList.Core.Models
{
    public class Item : Identity
    {
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

        /// <summary>
        /// ListItemId
        /// </summary>
        [Required(ErrorMessage = "ListItemId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The ListItemId must be in the range from 1 to 2147483647")]
        public int ListItemId { get; set; }
    }
}