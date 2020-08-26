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
using ToDoList.Core.Model;

namespace ToDoList.API.Controllers
{
    [Produces("application/json")]
    [ApiVersion(version: "1.0", Deprecated = false)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TodoListItemController : ControllerBase
    {
        private readonly ILogger<TodoListItemController> _logger;
        private readonly ITodoListItemFilterService _filter;
        private readonly ITodoListItemRepository _repo;

        public TodoListItemController(ITodoListItemRepository repo, ILogger<TodoListItemController> logger, ITodoListItemFilterService filter)
        {
            _logger = logger;
            _filter = filter;
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Enumerating TodoListItem by page
        /// </summary>
        /// <param name="queryParams">AG set of optional filtering and pagination parameters</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v{version}​/[controller]?{pageSize=10&amp;pageIndex=1&amp;title=title}
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<TodoListItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<TodoListItem>>> ItemsAsync([FromQuery] TodoListItemQueryParameters queryParams)
        {
            var todoListItems = _repo.GetQueryable();

            (IQueryable<TodoListItem> resultPages, long todoListItems) filteredValue = await _filter.FilteredByName(todoListItems, queryParams);

            var model = new PaginatedItemsViewModel<TodoListItem>(queryParams.PageIndex, queryParams.PageSize, filteredValue.todoListItems, await filteredValue.resultPages.ToListAsync());
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
        ///     GET /api/v{version}/[controller]/{id}
        /// </remarks>
        /// <response code="404">If the item is not found</response>
        [HttpGet("{id:int}", Name = nameof(GetTodoListItemAsync))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(TodoListItem), StatusCodes.Status200OK)]
        public async Task<ActionResult<TodoListItem>> GetTodoListItemAsync(int id)
        {
            var todoListItem = await _repo.GetByIdAsync(id);

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
        ///     POST /api/v{version}/[controller]/
        ///     {
        ///        "title": "Title",
        ///        "description": "Description"
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(TodoListItem), StatusCodes.Status201Created)]
        public async Task<ActionResult<TodoListItem>> CreateTodoListItemAsync([FromBody] TodoListItem todoListItem)
        {
            _repo.Add(todoListItem);
            await _repo.SaveAsync();

            return CreatedAtRoute(nameof(GetTodoListItemAsync), new { id = todoListItem.Id }, todoListItem);
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
        ///     PUT /api/v{version}/[controller]/{id}/
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

            _repo.Update(todoListItem);

            try
            {
                await _repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repo.GetQueryable().Any(t => t.Id == todoListItem.Id))
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
        ///     DELETE /api/v{version}/[controller]/{id}
        /// </remarks>
        /// <response code="404">If the item is not found</response>
        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoListItem>> DeleteTodoListItemAsync(int id)
        {
            var todoListItem = await _repo.GetQueryable().SingleOrDefaultAsync(i => i.Id == id);

            if (todoListItem == null)
            {
                _logger.LogError($"TodoListItem with id {id} not found.");
                return NotFound();
            }

            _repo.Remove(todoListItem);
            await _repo.SaveAsync();

            return todoListItem;
        }
    }
}