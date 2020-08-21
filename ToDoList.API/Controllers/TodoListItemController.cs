﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Extensions;
using ToDoList.API.Infrastructure;
using ToDoList.API.ViewModel;
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

        // GET api/[controller][?PageSize=10&PageIndex=1&Title=text]
        [HttpGet]
        public async Task<ActionResult<PaginatedItemsViewModel<TodoListItem>>> ItemsAsync([FromQuery] TodoListItemQueryParameters queryParameters)
        {
            IQueryable<TodoListItem> todoListItems = _todoListContext.TodoListItems;

            todoListItems = todoListItems.WhereIf(
                x => x.Title.Trim().Contains(queryParameters.Title.Trim()),
                () => !string.IsNullOrWhiteSpace(queryParameters.Title));

            var totalitems = await todoListItems.LongCountAsync();

            todoListItems = todoListItems
                .OrderBy(t => t.Title)
                .Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
                .Take(queryParameters.PageSize);

            var model = new PaginatedItemsViewModel<TodoListItem>(queryParameters.PageIndex, queryParameters.PageSize, totalitems, await todoListItems.ToListAsync());
            return Ok(model);
            //return Ok(await todoListItems.ToListAsync());
        }

        // GET api/[controller]/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TodoListItem>> GetTodoListItem(int id)
        {
            var todoListItem = await _todoListContext.TodoListItems.FindAsync(id);

            if (todoListItem == null)
            {
                _logger.LogError($"TodoListItem with id {id} not found.");
                return NotFound();
            }

            return Ok(todoListItem);
        }

        // POST api/[controller]/
        [HttpPost]
        public async Task<ActionResult<TodoListItem>> CreateTodoListItemAsync([FromBody] TodoListItem todoListItem)
        {
            _todoListContext.TodoListItems.Add(todoListItem);
            await _todoListContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoListItem), new { id = todoListItem.Id }, todoListItem);
        }

        // PUT api/[controller]/
        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> UpdateTodoListItemAsync([FromRoute] int id, [FromBody] TodoListItem todoListItem)
        {
            if (id != todoListItem.Id)
            {
                _logger.LogError($"TodoListItem.Id {todoListItem.Id} does not match the Id {id}.");
                return BadRequest();
            }

            _todoListContext.Entry(todoListItem).State = EntityState.Modified;

            try
            {
                await _todoListContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_todoListContext.TodoListItems.Find(todoListItem.Id) == null)
                {
                    _logger.LogError($"TodoListItem with id {id} not found.");
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE api/[controller]/
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<TodoListItem>> DeleteTodoListItemAsync(int id)
        {
            var todoListItem = await _todoListContext.TodoListItems.SingleOrDefaultAsync(i => i.Id == id);

            if (todoListItem == null)
            {
                _logger.LogError($"TodoListItem with id {id} not found.");
                return NotFound();
            }

            _todoListContext.TodoListItems.Remove(todoListItem);
            await _todoListContext.SaveChangesAsync();

            return todoListItem;
        }
    }
}