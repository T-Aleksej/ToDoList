using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ToDoList.API.Controllers;
using ToDoList.API.Infrastructure;
using ToDoList.API.Infrastructure.Filters;
using ToDoList.API.Infrastructure.Filters.Interfaces;
using ToDoList.API.ViewModel;
using ToDoList.Core.Context;
using ToDoList.Core.Model;
using ToDoList.Core.Repositories;
using Xunit;

namespace ToDoList.API.UnitTests.Application
{
    public class TodoListItemControllerTest
    {
        private readonly DbContextOptions<TodoListContext> _dbOptions;
        private readonly Mock<ILogger<TodoListItemController>> _loggerMock;

        public TodoListItemControllerTest()
        {
            _loggerMock = new Mock<ILogger<TodoListItemController>>();

            _dbOptions = new DbContextOptionsBuilder<TodoListContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            using (var dbContext = new TodoListContext(_dbOptions))
            {
                dbContext.AddRange(GetFakeTodoListItems());
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task Get_TodoListItems_Should_Succeed()
        {
            //Arrange
            var totalitems = 1;
            var pageIndex = 1;
            var pageSize = 15;
            var itemsInPage = 1;
            Mock<TodoListItemQueryParameters> fakeTodoListItemQueryParameters = new Mock<TodoListItemQueryParameters>();
            var repo = new TodoListItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoListItemController = new TodoListItemController(repo, _loggerMock.Object, filter);
            var actionResult = await todoListItemController.ItemsAsync(fakeTodoListItemQueryParameters.Object, pageIndex, pageSize);

            //Assert
            Assert.IsType<ActionResult<PaginatedItemsViewModel<TodoListItem>>>(actionResult);
            var viewModel = Assert.IsAssignableFrom<PaginatedItemsViewModel<TodoListItem>>(((ObjectResult)actionResult.Result).Value);
            Assert.Equal(totalitems, viewModel.TotalCount);
            Assert.Equal(pageIndex, viewModel.PageIndex);
            Assert.Equal(pageSize, viewModel.PageSize);
            Assert.Equal(itemsInPage, viewModel.Data.Count());
        }

        [Fact]
        public async Task Get_TodoListItem_Should_Succeed()
        {
            //Arrange
            int todoListItemId = 1;
            var repo = new TodoListItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoListItemController = new TodoListItemController(repo, _loggerMock.Object, filter);
            var actionResult = await todoListItemController.GetTodoListItemAsync(todoListItemId);

            //Assert
            Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)HttpStatusCode.OK);
            Assert.Equal((((ObjectResult)actionResult.Result).Value as TodoListItem).Id, todoListItemId);
        }

        [Fact]
        public async Task Get_TodoListItem_Should_Return_404_If_No_Data_Exist()
        {
            //Arrange
            int notExistTodoListItemId = 2;
            var repo = new TodoListItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoListItemController = new TodoListItemController(repo, _loggerMock.Object, filter);
            var actionResult = await todoListItemController.GetTodoListItemAsync(notExistTodoListItemId);

            //Assert
            Assert.Equal((actionResult.Result as NotFoundResult).StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_TodoListItem_Should_Succeed()
        {
            //Arrange
            int todoListItemId = 3;
            var repo = new TodoListItemRepository(new TodoListContext(_dbOptions));
            var fakeTodoListItem = GetTodoListItemFake(todoListItemId);
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoListItemController = new TodoListItemController(repo, _loggerMock.Object, filter);
            var actionResult = await todoListItemController.CreateTodoListItemAsync(fakeTodoListItem);

            //Assert
            Assert.Equal((actionResult.Result as ObjectResult).StatusCode, (int)HttpStatusCode.Created);
            //Assert.Equal((((ObjectResult)actionResult.Result).Value as TodoListItem).Id, todoListItemId);
            Assert.Equal(((CreatedResult)actionResult.Result).Value, todoListItemId);

        }

        [Fact]
        public async Task Put_TodoListItem_Should_Succeed()
        {
            //Arrange
            int todoListItemId = 1;
            var repo = new TodoListItemRepository(new TodoListContext(_dbOptions));
            var fakeTodoListItem = GetTodoListItemFake(todoListItemId);
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoListItemController = new TodoListItemController(repo, _loggerMock.Object, filter);
            var actionResult = await todoListItemController.UpdateTodoListItemAsync(todoListItemId, fakeTodoListItem);

            //Assert
            Assert.Equal((actionResult as NoContentResult).StatusCode, (int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Put_TodoListItem_Should_Return_400_If_Data_Wrong()
        {
            //Arrange
            int todoListItemId = 1;
            int wrongTodoListItemId = 2;
            var repo = new TodoListItemRepository(new TodoListContext(_dbOptions));
            var fakeTodoListItem = GetTodoListItemFake(todoListItemId);
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoListItemController = new TodoListItemController(repo, _loggerMock.Object, filter);
            var actionResult = await todoListItemController.UpdateTodoListItemAsync(wrongTodoListItemId, fakeTodoListItem);

            //Assert
            Assert.Equal((actionResult as BadRequestResult).StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_TodoListItem_Should_Return_404_If_No_Data_Exist()
        {
            //Arrange
            int notExistTodoListItemId = 2;
            var repo = new TodoListItemRepository(new TodoListContext(_dbOptions));
            var fakeTodoListItem = GetTodoListItemFake(notExistTodoListItemId);
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoListItemController = new TodoListItemController(repo, _loggerMock.Object, filter);
            var actionResult = await todoListItemController.UpdateTodoListItemAsync(notExistTodoListItemId, fakeTodoListItem);

            //Assert
            Assert.Equal((actionResult as NotFoundResult).StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_TodoListItem_Should_Succeed()
        {
            //Arrange
            int todoListItemId = 1;
            var repo = new TodoListItemRepository(new TodoListContext(_dbOptions));
            var fakeTodoListItem = GetTodoListItemFake(todoListItemId);
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoListItemController = new TodoListItemController(repo, _loggerMock.Object, filter);
            var actionResult = await todoListItemController.DeleteTodoListItemAsync(todoListItemId);

            //Assert
            Assert.Equal(actionResult.Value.Id, todoListItemId);
        }

        [Fact]
        public async Task Delete_TodoListItem_Should_Return_404_If_No_Data_Exist()
        {
            //Arrange
            int notExistTodoListItemId = 2;
            var repo = new TodoListItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoListItemController = new TodoListItemController(repo, _loggerMock.Object, filter);
            var actionResult = await todoListItemController.DeleteTodoListItemAsync(notExistTodoListItemId);

            //Assert
            Assert.Equal((actionResult.Result as NotFoundResult).StatusCode, (int)HttpStatusCode.NotFound);
        }

        private List<TodoListItem> GetFakeTodoListItems()
        {
            return new List<TodoListItem>()
            {
                new TodoListItem()
                {
                    Id = 1,
                    Title = "Title",
                    Description = "Description"
                }
            };
        }
        private TodoListItem GetTodoListItemFake(int todoItemId)
        {
            return new TodoListItem()
            {
                Id = todoItemId,
                Title = "TodoListItemFake",
                Description = "Description"
            };
        }
    }
}