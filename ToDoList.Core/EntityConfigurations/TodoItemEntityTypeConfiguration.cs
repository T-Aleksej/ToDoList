using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Core.Entities;

namespace ToDoList.Core.EntityConfigurations
{
    class TodoItemEntityTypeConfiguration : IEntityTypeConfiguration<TodoItem>
    {
        public void Configure(EntityTypeBuilder<TodoItem> builder)
        {
            builder.HasOne(t => t.TodoListItem).WithMany(t => t.TodoItems).HasForeignKey(t => t.TodoListItemId);

            builder.Property(t => t.Title).IsRequired(true).HasMaxLength(50);
            builder.Property(t => t.Content).IsRequired(true).HasMaxLength(1000);
            builder.Property(t => t.DuetoDateTime).HasColumnType("date").IsRequired(true);
            builder.Property(t => t.Done).IsRequired(true);
        }
    }
}