using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ToDoList.API.Infrastructure;
using ToDoList.API.Infrastructure.Filters.Interfaces;
using ToDoList.API.ViewModel;
using ToDoList.Core.Entities;
using ToDoList.Core.Repositories.Interfaces;
using ToDoList.Core.Models;

namespace ToDoList.API.Controllers
{
    [Produces("application/json")]
    [ApiVersion(version: "1.0", Deprecated = false)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TodoListItemController : GenericController<TodoListItem, ListItem>
    {
        private readonly ILogger<TodoListItemController> _logger;
        private readonly ITodoListItemRepository _repo;

        public TodoListItemController(ITodoListItemRepository repo, ILogger<TodoListItemController> logger, IFilterWrapper filter, IMapper mapper)
            : base(repo, logger, filter, mapper)
        {
            _logger = logger;
            _repo = repo;
        }

        /// <summary>
        /// Enumerating TodoListItem by page
        /// </summary>
        /// <param name="queryParams">Filter parametr</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>Paginated todoListItems</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v{version}​/[controller]?{pageSize=10&amp;pageIndex=1&amp;title=title}
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<ListItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<ListItem>>> ItemsAsync([FromQuery] TodoListItemQueryParameters queryParams, int pageIndex = 1, [Range(1, 100)] int pageSize = 10)
        {
            return await GetItems(_repo.GetQueryable(), queryParams, pageIndex, pageSize);
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
        [ProducesResponseType(typeof(ListItem), StatusCodes.Status200OK)]
        public async Task<ActionResult<ListItem>> GetTodoListItemAsync(int id) => await GetItem(id);

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
        [ProducesResponseType(typeof(ListItem), StatusCodes.Status201Created)]
        public async Task<ActionResult<ListItem>> CreateTodoListItemAsync([FromBody] ListItem todoListItem) => await Create(todoListItem);

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
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateTodoListItemAsync([FromRoute] int id, [FromBody] ListItem todoListItem) => await Updat(id, todoListItem);

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
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ListItem>> DeleteTodoListItemAsync(int id) => await Delete(id);
    }
}