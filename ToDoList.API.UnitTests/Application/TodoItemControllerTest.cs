﻿using Microsoft.AspNetCore.Mvc;
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
using ToDoList.Core.Entities;
using ToDoList.Core.Repositories;
using Xunit;
using AutoMapper;
using ToDoList.Core.Models;

namespace ToDoList.API.UnitTests.Application
{
    public class TodoItemControllerTest
    {
        private readonly DbContextOptions<TodoListContext> _dbOptions;
        private readonly Mock<ILogger<TodoItemController>> _loggerMock;
        private readonly IMapper _mapper;

        public TodoItemControllerTest()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            _mapper = mapperConfig.CreateMapper();

            _loggerMock = new Mock<ILogger<TodoItemController>>();

            _dbOptions = new DbContextOptionsBuilder<TodoListContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            using (var dbContext = new TodoListContext(_dbOptions))
            {
                dbContext.AddRange(GetFakeTodoItems());
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task Get_TodoItems_Should_Succeed()
        {
            //Arrange
            var todoListItemId = 1;
            var totalitems = 1;
            var pageIndex = 1;
            var pageSize = 15;
            var itemsInPage = 1;
            Mock<TodoItemQueryParameters> fakeTodoItemQueryParameters = new Mock<TodoItemQueryParameters>();
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();


            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.ItemsAsync(todoListItemId, fakeTodoItemQueryParameters.Object, pageIndex, pageSize);

            //Assert
            Assert.IsType<ActionResult<PaginatedItemsViewModel<Item>>>(actionResult);
            var viewModel = Assert.IsAssignableFrom<PaginatedItemsViewModel<Item>>(((ObjectResult)actionResult.Result).Value);
            Assert.Equal(totalitems, viewModel.TotalCount);
            Assert.Equal(pageIndex, viewModel.PageIndex);
            Assert.Equal(pageSize, viewModel.PageSize);
            Assert.Equal(itemsInPage, viewModel.Data.Count());
        }

        [Fact]
        public async Task Get_TodoItems_Should_Return_404_If_No_Data_Exist()
        {
            //Arrange
            var notExistTodoListItemId = 2;
            var pageIndex = 1;
            var pageSize = 15;
            Mock<TodoItemQueryParameters> fakeTodoItemQueryParameters = new Mock<TodoItemQueryParameters>();
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.ItemsAsync(notExistTodoListItemId, fakeTodoItemQueryParameters.Object, pageIndex, pageSize);

            //Assert
            Assert.Equal((actionResult.Result as NotFoundResult).StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_TodoItem_Should_Succeed()
        {
            //Arrange
            int todoItemId = 1;
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.GetTodoItemAsync(todoItemId);

            //Assert
            Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)HttpStatusCode.OK);
            Assert.Equal((((ObjectResult)actionResult.Result).Value as Item).Id, todoItemId);
        }

        [Fact]
        public async Task Get_TodoItem_Should_Return_404_If_No_Data_Exist()
        {
            //Arrange
            int notExistTodoItemId = 2;
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.GetTodoItemAsync(notExistTodoItemId);

            //Assert
            Assert.Equal((actionResult.Result as NotFoundResult).StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_TodoItem_Should_Succeed()
        {
            //Arrange
            int todoItemId = 2;
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            var fakeTodoItem = GetItemFake(todoItemId);
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.CreateTodoItemAsync(fakeTodoItem);

            //Assert
            Assert.Equal((actionResult.Result as ObjectResult).StatusCode, (int)HttpStatusCode.Created);
            //Assert.Equal((((ObjectResult)actionResult.Result).Value as TodoItem).Id, todoItemId);
            Assert.Equal(((CreatedResult)actionResult.Result).Value, todoItemId);
        }

        [Fact]
        public async Task Put_TodoItem_Should_Succeed()
        {
            //Arrange
            int todoItemId = 1;
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            var fakeTodoItem = GetItemFake(todoItemId);
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.UpdateTodoItemAsync(todoItemId, fakeTodoItem);

            //Assert
            Assert.Equal((actionResult as NoContentResult).StatusCode, (int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Put_TodoItem_Should_Return_404_If_No_Data_Exist()
        {
            //Arrange
            int notExistTodoItemId = 5;
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            var fakeTodoItem = GetItemFake(notExistTodoItemId);
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.UpdateTodoItemAsync(notExistTodoItemId, fakeTodoItem);

            //Assert
            Assert.Equal((actionResult as NotFoundResult).StatusCode, (int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_TodoItem_Should_Return_400_If_Data_Wrong()
        {
            //Arrange
            int todoItemId = 1;
            int wrongTodoItemId = 2;
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            var fakeTodoItem = GetItemFake(todoItemId);
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.UpdateTodoItemAsync(wrongTodoItemId, fakeTodoItem);

            //Assert
            Assert.Equal((actionResult as BadRequestResult).StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_TodoItem_Should_Succeed()
        {
            //Arrange
            int todoItemId = 1;
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.DeleteTodoItemAsync(todoItemId);

            //Assert
            Assert.Equal(actionResult.Value.Id, todoItemId);
        }

        [Fact]
        public async Task Delete_TodoItem_Should_Return_404_If_No_Data_Exist()
        {
            //Arrange
            int notExistTodoItemId = 5;
            var repo = new TodoItemRepository(new TodoListContext(_dbOptions));
            IFilterWrapper filter = new FilterWrapper();

            // Act
            var todoItemController = new TodoItemController(repo, _loggerMock.Object, filter, _mapper);
            var actionResult = await todoItemController.DeleteTodoItemAsync(notExistTodoItemId);

            //Assert
            Assert.Equal((actionResult.Result as NotFoundResult).StatusCode, (int)HttpStatusCode.NotFound);
        }

        private List<TodoItem> GetFakeTodoItems()
        {
            return new List<TodoItem>()
            {
                new TodoItem()
                {
                    Id = 1,
                    TodoListItemId = 1,
                    Title = "Title",
                    Content = "Content",
                    Done = true,
                    DuetoDateTime = DateTime.Now
                }
            };
        }
        private Item GetItemFake(int todoItemId)
        {
            return new Item()
            {
                Id = todoItemId,
                //TodoListItemId = 1,
                ListItemId = 1,
                Title = "TodoItemFake",
                Content = "Content",
                Done = true,
                DuetoDateTime = DateTime.Now
            };
        }
    }
}