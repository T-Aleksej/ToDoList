﻿using Microsoft.EntityFrameworkCore;
using ToDoList.Core.EntityConfigurations;
using ToDoList.Core.Model;

namespace ToDoList.Core.Context
{
    public class TodoListContext : DbContext
    {
        public TodoListContext(DbContextOptions<TodoListContext> options) : base(options) { }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TodoListItem> TodoListItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TodoListItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new TodoItemEntityTypeConfiguration());
        }
    }
}