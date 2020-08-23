using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Infrastructure;
using ToDoList.API.Services;
using ToDoList.API.ViewModel;
using ToDoList.Core.Context;
using ToDoList.Core.Model;

namespace ToDoList.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListItemController : ControllerBase
    {
        private readonly ILogger<TodoListItemController> _logger;
        private readonly TodoListContext _todoListContext;
        private readonly ITodoListItemFilterService _filter;

        public TodoListItemController(TodoListContext context, ILogger<TodoListItemController> logger, ITodoListItemFilterService filter)
        {
            _logger = logger;
            _todoListContext = context ?? throw new ArgumentNullException(nameof(context));
            _filter = filter;
        }

        /// <summary>
        /// Enumerating TodoListItem by page
        /// </summary>
        /// <param name="queryParameters">AG set of optional filtering and pagination parameters</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/[controller]?{pageSize=10&amp;pageIndex=1&amp;title=title}
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<TodoListItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<TodoListItem>>> ItemsAsync([FromQuery] TodoListItemQueryParameters queryParameters)
        {
            IQueryable<TodoListItem> todoListItems = _todoListContext.TodoListItems;

            (IQueryable<TodoListItem> resultPages, long todoListItems) filteredValue = await _filter.FilteredByName(todoListItems, queryParameters);

            var model = new PaginatedItemsViewModel<TodoListItem>(queryParameters.PageIndex, queryParameters.PageSize, filteredValue.todoListItems, await filteredValue.resultPages.ToListAsync());
            return Ok(model);
        }

        /// <summary>
        /// TodoListItem matching passed ID
        /// </summary>
        /// <param name="id">The TodoListItem id</param>
        /// <returns>Concretely TodoListItem</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/[controller]/{id}
        /// </remarks>
        /// <response code="404">If the item is not found</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TodoListItem), StatusCodes.Status200OK)]
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

        /// <summary>
        /// Add a specific TodoListItem.
        /// </summary>
        /// <param name="todoListItem">The TodoListItem</param>
        /// <returns>A newly aded TodoListItem</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/[controller]/
        ///     {
        ///        "title": "Title",
        ///        "description": "Description"
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(TodoListItem), StatusCodes.Status201Created)]
        public async Task<ActionResult<TodoListItem>> CreateTodoListItemAsync([FromBody] TodoListItem todoListItem)
        {
            _todoListContext.TodoListItems.Add(todoListItem);
            await _todoListContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoListItem), new { id = todoListItem.Id }, todoListItem);
        }

        /// <summary>
        /// Change a specific TodoListItem.
        /// </summary>
        /// <param name="id">The TodoListItem id</param>
        /// <param name="todoListItem">The TodoListItem</param>
        /// <returns>Status codes 204 No Content</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/[controller]/{id}/
        ///     {
        ///        "id": 1,
        ///        "title": "Title",
        ///        "description": "Description"
        ///     }
        /// </remarks>
        /// <response code="400">If id is not equal to TodoListItem.Id</response>
        /// <response code="404">If the item is not found</response>
        [HttpPut]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Deletes a specific TodoListItem.
        /// </summary>
        /// <param name="id">The TodoListItem id</param>
        /// <returns>A newly deleted TodoListItem</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/[controller]/{id}
        /// </remarks>
        /// <response code="404">If the item is not found</response>
        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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