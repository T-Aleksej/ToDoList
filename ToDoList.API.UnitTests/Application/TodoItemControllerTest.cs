using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ToDoList.API.Controllers;
using ToDoList.Core.Context;
using ToDoList.Core.Model;
using Xunit;

namespace ToDoList.API.UnitTests.Application
{
    public class TodoItemControllerTest
    {
        private readonly DbContextOptions<TodoListContext> _dbOptions;
        private readonly Mock<ILogger<TodoItemController>> _loggerMock;

        public TodoItemControllerTest()
        {
            _loggerMock = new Mock<ILogger<TodoItemController>>();

            _dbOptions = new DbContextOptionsBuilder<TodoListContext>()
                .UseInMemoryDatabase(databaseName: "ToDoList").Options;

            using (var dbContext = new TodoListContext(_dbOptions))
            {
                dbContext.Database.EnsureDeleted();

                dbContext.AddRange(GetFakeTodoItem());
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task Get_TodoItem_Success()
        {
            //Arrange
            int todoItemId = 1;
            var todoListContext = new TodoListContext(_dbOptions);

            // Act
            var todoItemController = new TodoItemController(todoListContext, _loggerMock.Object);
            var actionResult = await todoItemController.GetTodoItem(todoItemId);

            //Assert
            Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)HttpStatusCode.OK);
            Assert.Equal((((ObjectResult)actionResult.Result).Value as TodoItem).Id, todoItemId);
            //expected, T actual)
        }

        [Fact]
        public async Task Post_TodoItem_Success()
        {
            //Arrange
            int todoItemId = 2;
            var todoListContext = new TodoListContext(_dbOptions);
            var fakeTodoItem = GetTodoItemFake(todoItemId);

            // Act
            var todoItemController = new TodoItemController(todoListContext, _loggerMock.Object);
            var actionResult = await todoItemController.CreateTodoItemAsync(fakeTodoItem);

            //Assert
            Assert.Equal((actionResult.Result as ObjectResult).StatusCode, (int)HttpStatusCode.Created);
            Assert.Equal((((ObjectResult)actionResult.Result).Value as TodoItem).Id, todoItemId);
        }

        [Fact]
        public async Task Put_TodoItem_Success()
        {
            //Arrange
            int todoItemId = 1;
            var todoListContext = new TodoListContext(_dbOptions);
            var fakeTodoItem = GetTodoItemFake(todoItemId);

            // Act
            var todoItemController = new TodoItemController(todoListContext, _loggerMock.Object);
            var actionResult = await todoItemController.UpdateTodoItemAsync(todoItemId, fakeTodoItem);

            //Assert
            Assert.Equal((actionResult as NoContentResult).StatusCode, (int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_TodoItem_Success()
        {
            //Arrange
            int todoItemId = 1;
            var todoListContext = new TodoListContext(_dbOptions);
            var fakeTodoItem = GetTodoItemFake(todoItemId);

            // Act
            var todoItemController = new TodoItemController(todoListContext, _loggerMock.Object);
            var actionResult = await todoItemController.DeleteTodoItemAsync(todoItemId);

            //Assert
            Assert.Equal(actionResult.Value.Id, todoItemId);
        }

        private List<TodoItem> GetFakeTodoItem()
        {
            return new List<TodoItem>()
            {
                new TodoItem()
                {
                    Id = 1,
                    //TodoListItem = new TodoListItem() { Id = 1, Title = "Title", Description = "Description" },
                    Title = "Title",
                    Content = "Content",
                    Done = true,
                    DuetoDateTime = DateTime.Now
                }
            };
        }
        private TodoItem GetTodoItemFake(int todoItemId)
        {
            return new TodoItem()
            {
                Id = todoItemId,
                TodoListItemId = 1,
                Title = "TodoItemFake",
                Content = "Content",
                Done = true,
                DuetoDateTime = DateTime.Now
            };
        }
    }
}