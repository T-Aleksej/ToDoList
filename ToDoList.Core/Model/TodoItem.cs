using Newtonsoft.Json;
using System;

namespace ToDoList.Core.Model
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DuetoDateTime { get; set; }
        public bool Done { get; set; }

        public int TodoListItemId { get; set; }
        [JsonIgnore]  public TodoListItem TodoListItem { get; set; }
    }
}