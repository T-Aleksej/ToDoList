﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToDoList.Core.Entities.Base;

namespace ToDoList.Core.Models
{
    public class ListItem: Identity
    {
        /// <summary>
        /// The title of the TodoListItem
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(50, ErrorMessage = "Title can't be longer than 50 characters")]
        public string Title { get; set; }

        /// <summary>
        /// The description of the TodoListItem
        /// </summary>
        [MaxLength(100, ErrorMessage = "Description can't be longer than 100 characters")]
        public string Description { get; set; }
    }
}