using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Infrastructure;
using ToDoList.API.Infrastructure.Filters.Interfaces;
using ToDoList.API.ViewModel;
using ToDoList.Core.Entities;
using ToDoList.Core.Models;
using ToDoList.Core.Repositories.Interfaces;

namespace ToDoList.API.Controllers
{
    [Produces("application/json")]
    [ApiVersion(version: "1.0", Deprecated = false)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TodoItemController : GenericController<TodoItem, Item>
    {
        private readonly ILogger<TodoItemController> _logger;
        private readonly ITodoItemRepository _repo;

        public TodoItemController(ITodoItemRepository repo, ILogger<TodoItemController> logger, IFilterWrapper filter, IMapper mapper)
            : base(repo, logger, filter, mapper)
        {
            _logger = logger;
            _repo = repo;
        }

        /// <summary>
        /// Enumerating TodoItem by page
        /// </summary>
        /// <param name="todoListItem">TodoListItem Id</param>
        /// <param name="queryParam"></param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v{version}/[controller]/todoListItem/{todoListItem}/todoItems?{pageIndex=10&amp;pageSize=1&amp;title=title&amp;date=2020-08-20&amp;isComplete=true}
        /// </remarks>
        /// <response code="404">If the items is not found</response>
        [HttpGet("todoListItem/{todoListItem:int}/todoItems")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedItemsViewModel<Item>>> ItemsAsync([FromRoute] int todoListItem, [FromQuery] TodoItemQueryParameters queryParam, int pageIndex = 1, [Range(1, 100)] int pageSize = 10)
        {
            return await GetItems(_repo.GetQueryable().Where(t => t.TodoListItemId == todoListItem), queryParam, pageIndex, pageSize);
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
        [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
        public async Task<ActionResult<Item>> GetTodoItemAsync(int id) => await GetItem(id);

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
        ///        "done": true,
        ///        "ListItemId": 1
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(Item), StatusCodes.Status201Created)]
        public async Task<ActionResult<Item>> CreateTodoItemAsync([FromBody] Item todoItem) => await Create(todoItem);

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
        ///        "done": true,
        ///        "ListItemId": 1
        ///     }
        /// </remarks>
        /// <response code="400">If id is not equal to TodoItem.Id</response>
        /// <response code="404">If the item is not found</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateTodoItemAsync([FromRoute] int id, [FromBody] Item todoItem) => await Updat(id, todoItem);

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
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Item>> DeleteTodoItemAsync(int id) => await Delete(id);
    }
}