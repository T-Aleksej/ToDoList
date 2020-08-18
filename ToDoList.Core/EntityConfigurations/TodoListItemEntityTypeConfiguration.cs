using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Core.Model;

namespace ToDoList.Core.EntityConfigurations
{
    class TodoListItemEntityTypeConfiguration : IEntityTypeConfiguration<TodoListItem>
    {
        public void Configure(EntityTypeBuilder<TodoListItem> builder)
        {
            builder.Property(t => t.Title).IsRequired(true).HasMaxLength(50);
            builder.Property(t => t.Description).IsRequired(false).HasMaxLength(100);
        }
    }
}