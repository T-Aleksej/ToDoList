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

        // GET api/[controller][?TodoListItem=1&PageSize=10&PageIndex=1&Title=text&IsComplete=true&Date=2020-08-20]
        [HttpGet]
        public async Task<ActionResult> ItemsAsync([FromQuery] TodoItemQueryParameters queryParameters)
        {
            IQueryable<TodoItem> todoItems = _todoListContext.TodoItems.Where(t => t.TodoListItemId == queryParameters.TodoListItem);

            if (await todoItems.CountAsync() == 0)
            {
                _logger.LogError($"TodoItem with id {queryParameters.TodoListItem} not found.");
                return NotFound();
            }

            todoItems = todoItems.WhereIf(
                x => x.Title.Trim().Contains(queryParameters.Title.Trim()),
                () => !string.IsNullOrWhiteSpace(queryParameters.Title));

            todoItems = todoItems.WhereIf(
                x => x.Done == queryParameters.IsComplete.Value,
                () => queryParameters.IsComplete.HasValue);

            todoItems = todoItems.WhereIf(
                x => x.DuetoDateTime.Date == queryParameters.Date.Value.Date,
                () => queryParameters.Date.HasValue);

            todoItems = todoItems
                .OrderBy(t => t.Title)
                .Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
                .Take(queryParameters.PageSize);

            return Ok(await todoItems.ToListAsync());
        }

        // GET api/[controller]/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var todoItem = await _todoListContext.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                _logger.LogError($"TodoItem with id {id} not found.");
                return NotFound();
            }

            return Ok(todoItem);
        }

        // POST api/[controller]/
        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateTodoItemAsync([FromBody] TodoItem todoItem)
        {
            _todoListContext.TodoItems.Add(todoItem);
            await _todoListContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // PUT api/[controller]/
        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> UpdateTodoItemAsync([FromRoute] int id, [FromBody] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                _logger.LogError($"TodoItem.Id {todoItem.Id} does not match the Id {id}.");
                return BadRequest();
            }

            _todoListContext.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _todoListContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_todoListContext.TodoListItems.Find(todoItem.Id) == null)
                {
                    _logger.LogError($"TodoItem with id {id} not found.");
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE api/[controller]/
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItemAsync(int id)
        {
            var todoItem = await _todoListContext.TodoItems.SingleOrDefaultAsync(i => i.Id == id);

            if (todoItem == null)
            {
                _logger.LogError($"TodoItem with id {id} not found.");
                return NotFound();
            }

            _todoListContext.TodoItems.Remove(todoItem);
            await _todoListContext.SaveChangesAsync();

            return todoItem;
        }
    }
}