using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Infrastructure;
using ToDoList.API.Repositories;
using ToDoList.API.Services;
using ToDoList.API.ViewModel;
using ToDoList.Core.Context;
using ToDoList.Core.Model;

namespace ToDoList.API.Controllers
{
    [Produces("application/json")]
    [ApiVersion(version: "1.0", Deprecated = false)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly ILogger<TodoItemController> _logger;
        private readonly ITodoItemFilterService _filter;
        private readonly ITodoItemRepository _repo;

        public TodoItemController(ITodoItemRepository repo, ILogger<TodoItemController> logger, ITodoItemFilterService filter)
        {
            _logger = logger;
            _filter = filter;
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Enumerating TodoItem by page
        /// </summary>
        /// <param name="queryParameters">AG set of optional filtering and pagination parameters</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v{version}/[controller]?{pageIndex=10&amp;pageSize=1&amp;todoListItem=1&amp;title=title&amp;date=2020-08-20&amp;isComplete=true}
        /// </remarks>
        /// <response code="404">If the items is not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<TodoItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedItemsViewModel<TodoItem>>> ItemsAsync([FromQuery] TodoItemQueryParameters queryParameters)
        {
            var todoItems = _repo.GetQueryable().Where(t => t.TodoListItemId == queryParameters.TodoListItem);

            if (await todoItems.CountAsync() == 0)
            {
                _logger.LogError($"TodoItem with id {queryParameters.TodoListItem} not found.");
                return NotFound();
            }

            (IQueryable<TodoItem> resultPages, long todoListItems) filteredValue = await _filter.FilteredByNameAndDateAndComplete(todoItems, queryParameters);

            var model = new PaginatedItemsViewModel<TodoItem>(queryParameters.PageIndex, queryParameters.PageSize, filteredValue.todoListItems, await filteredValue.resultPages.ToListAsync());
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
        ///     GET /api/v{version}/[controller]/{id}
        /// </remarks>
        /// <response code="404">If the item is not found</response>
        [HttpGet("{id:int}", Name = nameof(GetTodoItemAsync))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
        public async Task<ActionResult<TodoItem>> GetTodoItemAsync(int id)
        {
            var todoItem = await _repo.GetByIdAsync(id);

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
        ///     POST /api/v{version}/[controller]/
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
            _repo.Add(todoItem);
            await _repo.SaveAsync();

            return CreatedAtRoute(nameof(GetTodoItemAsync), new { id = todoItem.Id }, todoItem);
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
        ///     PUT /api/v{version}/[controller]/{id}
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

            _repo.Update(todoItem);

            try
            {
                await _repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repo.GetQueryable().Any(t => t.Id == todoItem.Id))
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
        ///     DELETE /api/v{version}/[controller]/{id}
        /// </remarks>
        /// <response code="404">If the item is not found</response>
        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItem>> DeleteTodoItemAsync(int id)
        {
            var todoItem = await _repo.GetQueryable().SingleOrDefaultAsync(i => i.Id == id);

            if (todoItem == null)
            {
                _logger.LogError($"TodoItem with id {id} not found.");
                return NotFound();
            }

            _repo.Remove(todoItem);
            await _repo.SaveAsync();

            return todoItem;
        }
    }
}