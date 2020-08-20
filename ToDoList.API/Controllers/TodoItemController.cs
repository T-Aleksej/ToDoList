using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Extensions;
using ToDoList.API.Infrastructure;
using ToDoList.Core.Context;
using ToDoList.Core.Model;

namespace ToDoList.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly ILogger<TodoItemController> _logger;
        private readonly TodoListContext _todoListContext;

        public TodoItemController(TodoListContext context, ILogger<TodoItemController> logger)
        {
            _logger = logger;
            _todoListContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET api/[controller]/items[?PageSize=10&PageIndex=1&Title=text&IsComplete=true&DateOfCompletion=01.01.2020]
        [HttpGet]
        [Route("items")]
        public async Task<ActionResult> ItemsAsync([FromQuery] TodoItemQueryParameters queryParameters)
        {
            IQueryable<TodoItem> todoItems = _todoListContext.TodoItems.Where(t => t.TodoListItemId == queryParameters.TodoListItemId);

            todoItems = todoItems.WhereIf(
                x => x.Title.Trim().Contains(queryParameters.Title.Trim()),
                () => !string.IsNullOrWhiteSpace(queryParameters.Title));

            todoItems = todoItems.WhereIf(
                x => x.Done == queryParameters.IsComplete.Value,
                () => queryParameters.IsComplete.HasValue);

            todoItems = todoItems.WhereIf(
                x => x.DuetoDateTime.Date == queryParameters.DateOfCompletion.Value.Date,
                () => queryParameters.DateOfCompletion.HasValue);

            todoItems = todoItems
                .OrderBy(t => t.Title)
                .Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
                .Take(queryParameters.PageSize);

            return Ok(await todoItems.ToListAsync());
        }

        // GET api/[controller]/items/<id>
        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<ActionResult> ItemByIdAsync(int id)
        {
            var todoItem = await _todoListContext.TodoItems.FindAsync(id);
            if (todoItem == null) return NotFound();

            return Ok(todoItem);
        }

        // POST api/[controller]/items/
        [HttpPost]
        [Route("items")]
        public async Task<ActionResult> CreateTodoItemAsync([FromBody] TodoItem todoItem)
        {
            _todoListContext.TodoItems.Add(todoItem);
            await _todoListContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ItemByIdAsync), new { id = todoItem.Id }, todoItem);
        }

        // PUT api/[controller]/items/
        [HttpPut]
        [Route("items")]
        public async Task<ActionResult> UpdateTodoItemAsync([FromBody] TodoItem todoItem)
        {
            _todoListContext.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _todoListContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_todoListContext.TodoListItems.Find(todoItem.Id) == null)
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE api/[controller]/<id>
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItemAsync(int id)
        {
            var todoItem = await _todoListContext.TodoItems.SingleOrDefaultAsync(i => i.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _todoListContext.TodoItems.Remove(todoItem);
            await _todoListContext.SaveChangesAsync();

            return todoItem;
        }
    }
}