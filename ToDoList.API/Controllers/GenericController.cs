using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Extensions;
using ToDoList.API.Infrastructure;
using ToDoList.API.Infrastructure.Filters.Interfaces;
using ToDoList.API.ViewModel;
using ToDoList.Core.Entities;
using ToDoList.Core.Entities.Base;
using ToDoList.Core.Models;
using ToDoList.Core.Repositories.Interfaces;

namespace ToDoList.API.Controllers
{
    public class GenericController<TEntity, TModel> : ControllerBase where TEntity : IHaveId
    {
        private readonly IGenericRepository<TEntity> _repo;
        private readonly ILogger _logger;
        private readonly IFilterWrapper _filter;
        private readonly IMapper _mapper;

        public GenericController(IGenericRepository<TEntity> repo, ILogger logger, IFilterWrapper filter, IMapper mapper)
        {
            _logger = logger;
            _repo = repo;
            _filter = filter;
            _mapper = mapper;
        }

        [NonAction]
        public async Task<ActionResult<PaginatedItemsViewModel<Item>>> GetItems(IQueryable<TodoItem> todoItems, TodoItemQueryParameters queryParam, int pageIndex, int pageSize)
        {

            if (await todoItems.CountAsync() == 0)
            {
                _logger.LogError($"TodoItem with id not found.");
                return NotFound();
            }

            todoItems = _filter.TodoItemFilter.Filtration(todoItems, queryParam);

            //var model = await todoItems.ToPagedListAsync(pageIndex, pageSize);

            (List<TodoItem> todoItems, int pageIndex, int pageSize, int totalCount) paging = await todoItems.ToPagedListAsync(pageIndex, pageSize);
            IList<Item> data = _mapper.Map<IList<Item>>(paging.todoItems);
            var model = new PaginatedItemsViewModel<Item>(data, paging.pageIndex, paging.pageSize, paging.totalCount);

            return Ok(model);
        }

        [NonAction]
        public async Task<ActionResult<PaginatedItemsViewModel<ListItem>>> GetItems(IQueryable<TodoListItem> todoListItems, TodoListItemQueryParameters queryParam, int pageIndex, int pageSize)
        {
            todoListItems = _filter.TodoListItemFilter.Filtration(todoListItems, queryParam);

            //var model = await todoListItems.ToPagedListAsync(pageIndex, pageSize);

            (List<TodoListItem> todoListItems, int pageIndex, int pageSize, int totalCount) paging = await todoListItems.ToPagedListAsync(pageIndex, pageSize);
            IList<ListItem> data = _mapper.Map<IList<ListItem>>(paging.todoListItems);
            var model = new PaginatedItemsViewModel<ListItem>(data, paging.pageIndex, paging.pageSize, paging.totalCount);

            return Ok(model);
        }

        [NonAction]
        public async Task<ActionResult<TModel>> GetItem(int id)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
            {
                _logger.LogError($"Item with id {id} not found.");
                return NotFound();
            }

            var model = _mapper.Map<TModel>(entity);
            return Ok(model);
        }

        [NonAction]
        public async Task<ActionResult<TModel>> Create(TModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model object");

            var entity = _mapper.Map<TEntity>(model);

            _repo.Add(entity);
            await _repo.SaveAsync();

            //return CreatedAtRoute("GetTodoListItemAsync", new { id = item.Id }, item);
            return Created(string.Format("{0}/{1}", Url?.Action(), entity.Id), entity.Id);
        }

        [NonAction]
        public async Task<ActionResult> Updat(int id, TModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var entity = _mapper.Map<TEntity>(model);

            if (id != entity.Id)
            {
                _logger.LogError($"ntity.Id {entity.Id} does not match the Id {id}.");
                return BadRequest();
            }
       
            _repo.Update(entity);

            try
            {
                await _repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repo.GetQueryable().Any(t => t.Id == entity.Id))
                {
                    _logger.LogError($"Item with id {id} not found.");
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [NonAction]
        public async Task<ActionResult<TModel>> Delete(int id)
        {
            var entity = await _repo.GetQueryable().SingleOrDefaultAsync(i => i.Id == id);

            if (entity == null)
            {
                _logger.LogError($"Entity with id {id} not found.");
                return NotFound();
            }

            _repo.Remove(entity);
            await _repo.SaveAsync();

            var model = _mapper.Map<TModel>(entity);

            return model;
        }
    }
}