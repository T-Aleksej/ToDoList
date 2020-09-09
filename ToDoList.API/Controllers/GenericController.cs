using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Extensions;
using ToDoList.API.Infrastructure;
using ToDoList.API.Infrastructure.Filters.Interfaces;
using ToDoList.API.ViewModel;
using ToDoList.Core.Entities;
using ToDoList.Core.Entities.Base;
using ToDoList.Core.Repositories.Interfaces;

namespace ToDoList.API.Controllers
{
    public class GenericController<T> : ControllerBase where T : IHaveId
    {
        public GenericController(IGenericRepository<T> repo, ILogger logger, IFilterWrapper filter)
        {
            _logger = logger;
            _repo = repo;
            _filter = filter;
        }

        private readonly IGenericRepository<T> _repo;
        private readonly ILogger _logger;
        private readonly IFilterWrapper _filter;

        [NonAction]
        public async Task<ActionResult<PaginatedItemsViewModel<TodoItem>>> GetItems(IQueryable<TodoItem> todoItems, TodoItemQueryParameters queryParam, int pageIndex, int pageSize)
        {

            if (await todoItems.CountAsync() == 0)
            {
                _logger.LogError($"TodoItem with id not found.");
                return NotFound();
            }

            todoItems = _filter.TodoItemFilter.Filtration(todoItems, queryParam);
            var model = await todoItems.ToPagedListAsync(pageIndex, pageSize);

            return Ok(model);
        }

        [NonAction]
        public async Task<ActionResult<PaginatedItemsViewModel<TodoListItem>>> GetItems(IQueryable<TodoListItem> todoListItems, TodoListItemQueryParameters queryParam, int pageIndex, int pageSize)
        {
            todoListItems = _filter.TodoListItemFilter.Filtration(todoListItems, queryParam);
            var model = await todoListItems.ToPagedListAsync(pageIndex, pageSize);

            return Ok(model);
        }

        [NonAction]
        public async Task<ActionResult<T>> GetItem(int id)
        {
            var todoItem = await _repo.GetByIdAsync(id);

            if (todoItem == null)
            {
                _logger.LogError($"Item with id {id} not found.");
                return NotFound();
            }

            return Ok(todoItem);
        }

        [NonAction]
        public async Task<ActionResult<T>> Create(T item)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model object");

            _repo.Add(item);
            await _repo.SaveAsync();

            //return CreatedAtRoute("GetTodoListItemAsync", new { id = item.Id }, item);
            return Created(string.Format("{0}/{1}", Url?.Action(), item.Id), item.Id);
        }

        [NonAction]
        public async Task<ActionResult> Updat(int id, T item)
        {
            if (id != item.Id)
            {
                _logger.LogError($"Item.Id {item.Id} does not match the Id {id}.");
                return BadRequest();
            }

            if (!ModelState.IsValid) return BadRequest("Invalid model object");

            _repo.Update(item);

            try
            {
                await _repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repo.GetQueryable().Any(t => t.Id == item.Id))
                {
                    _logger.LogError($"Item with id {id} not found.");
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [NonAction]
        public async Task<ActionResult<T>> Delete(int id)
        {

            var todoItem = await _repo.GetQueryable().SingleOrDefaultAsync(i => i.Id == id);

            if (todoItem == null)
            {
                _logger.LogError($"Item with id {id} not found.");
                return NotFound();
            }

            _repo.Remove(todoItem);
            await _repo.SaveAsync();

            return todoItem;
        }
    }
}