using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    public class TodoListItemController : ControllerBase
    {
        private readonly ILogger<TodoListItemController> _logger;
        private readonly TodoListContext _todoListContext;

        public TodoListItemController(TodoListContext context, ILogger<TodoListItemController> logger)
        {
            _logger = logger;
            _todoListContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET api/[controller]/items[?PageSize=10&PageIndex=1&Title=text]
        [HttpGet]
        [Route("items")]
        public async Task<ActionResult> ItemsAsync([FromQuery] TodoListItemQueryParameters queryParameters)
        {
            IQueryable<TodoListItem> todoListItems = _todoListContext.TodoListItems;

            todoListItems = todoListItems.WhereIf(
                x => x.Title.Trim().Contains(queryParameters.Title.Trim()),
                () => !string.IsNullOrWhiteSpace(queryParameters.Title));

            todoListItems = todoListItems
                .OrderBy(t => t.Title)
                .Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
                .Take(queryParameters.PageSize);

            return Ok(await todoListItems.ToListAsync());
        }

        // GET api/[controller]/items/<id>
        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<ActionResult> ItemByIdAsync(int id)
        {
            var todoListItem = await _todoListContext.TodoListItems.FindAsync(id);
            if (todoListItem == null) return NotFound();

            return Ok(todoListItem);
        }

        // POST api/[controller]/items/
        [HttpPost]
        [Route("items")]
        public async Task<ActionResult> CreateTodoListItemAsync([FromBody] TodoListItem todoListItem)
        {
            _todoListContext.TodoListItems.Add(todoListItem);
            await _todoListContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ItemByIdAsync), new { id = todoListItem.Id }, todoListItem);
        }

        // PUT api/[controller]/items/
        [HttpPut]
        [Route("items")]
        public async Task<ActionResult> UpdateTodoListItemAsync([FromBody] TodoListItem todoListItem)
        {
            _todoListContext.Entry(todoListItem).State = EntityState.Modified;

            try
            {
                await _todoListContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_todoListContext.TodoListItems.Find(todoListItem.Id) == null)
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
        public async Task<ActionResult<TodoListItem>> DeleteTodoListItemAsync(int id)
        {
            var todoListItem = await _todoListContext.TodoListItems.SingleOrDefaultAsync(i => i.Id == id);
            if (todoListItem == null)
            {
                return NotFound();
            }

            _todoListContext.TodoListItems.Remove(todoListItem);
            await _todoListContext.SaveChangesAsync();

            return todoListItem;
        }
    }
}