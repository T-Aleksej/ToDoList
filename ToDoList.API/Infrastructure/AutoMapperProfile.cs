using AutoMapper;
using ToDoList.Core.Entities;
using ToDoList.Core.Models;

namespace ToDoList.API.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TodoListItem, ListItem>();
            CreateMap<TodoItem, Item>().ForMember(x => x.ListItemId, x => x.MapFrom(m => m.TodoListItemId));

            CreateMap<ListItem, TodoListItem>();
            CreateMap<Item, TodoItem>().ForMember(x => x.TodoListItemId, x => x.MapFrom(m => m.ListItemId));
        }
    }
}