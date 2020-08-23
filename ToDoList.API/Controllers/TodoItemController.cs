using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Produces("application/json")]
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

        /// <summary>
        /// Enumerating TodoItem by page
        /// </summary>
        /// <param name="queryParameters">AG set of optional filtering and pagination parameters</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/[controller]?{pageIndex=10&amp;pageSize=1&amp;todoListItem=1&amp;title=title&amp;date=2020-08-20&amp;isComplete=true}
        /// </remarks>
        /// <response code="404">If the items is not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<TodoItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedItemsViewModel<TodoItem>>> ItemsAsync([FromQuery] TodoItemQueryParameters queryParameters)
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

            var totalitems = await todoItems.LongCountAsync();

            todoItems = todoItems
                .OrderBy(t => t.Title)
                .Skip(queryParameters.PageSize * (queryParameters.PageIndex - 1))
                .Take(queryParameters.PageSize);

            var model = new PaginatedItemsViewModel<TodoItem>(queryParameters.PageIndex, queryParameters.PageSize, totalitems, await todoItems.ToListAsync());
            return Ok(model);
        }

        /// <summary>
        /// TodoItem matching passed ID
        /// </summary>
        /// <param name="id">The TodoItem id</param>
        /// <returns>Concretely TodoItem</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/[controller]/{id}
        /// </remarks>
        /// <response code="404">If the item is not found</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
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

        /// <summary>
        /// Add a specific TodoItem.
        /// </summary>
        /// <param name="todoItem">The TodoItem</param>
        /// <returns>A newly aded TodoItem</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/[controller]/
        ///     {
        ///        "title": "Title",
        ///        "content": "Description",
        ///        "duetoDateTime": 2020-08-20,
        ///        "done": true
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(TodoItem), StatusCodes.Status201Created)]
        public async Task<ActionResult<TodoItem>> CreateTodoItemAsync([FromBody] TodoItem todoItem)
        {
            _todoListContext.TodoItems.Add(todoItem);
            await _todoListContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        /// <summary>
        /// Change a specific TodoItem.
        /// </summary>
        /// <param name="id">The TodoItem id</param>
        /// <param name="todoItem">The TodoItem</param>
        /// <returns>Status codes 204 No Content</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/[controller]/{id}
        ///     {
        ///        "id": 1,
        ///        "title": "Title",
        ///        "content": "Description",
        ///        "duetoDateTime": 2020-08-20,
        ///        "done": true
        ///     }
        /// </remarks>
        /// <response code="400">If id is not equal to TodoItem.Id</response>
        /// <response code="404">If the item is not found</response>
        [HttpPut]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id">The TodoItem id</param>
        /// <returns>A newly deleted TodoItem</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/[controller]/{id}
        /// </remarks>
        /// <response code="404">If the item is not found</response>
        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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