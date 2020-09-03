﻿namespace ToDoList.Core.Model.Base
{
    /// <summary>
    /// Identifier
    /// </summary>
    public abstract class Identity : IHaveId
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }
    }
}