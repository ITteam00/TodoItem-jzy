using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToDoList.Api.Models;
using Xunit;
using static sun.security.provider.ConfigFile;

namespace ToDoList.Api.ApiTests
{
    public class CreateOneTodoItemTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IMongoCollection<ToDoItem> _mongoCollection;

        public CreateOneTodoItemTest(WebApplicationFactory<Program> factory)
        {
            //_factory = factory;
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseContentRoot("C:\\Users\\zjiang10\\Desktop\\agile\\ToDoList.Api");
            });
            _client = _factory.CreateClient();

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var mongoDatabase = mongoClient.GetDatabase("TodoTestStore");
            _mongoCollection = mongoDatabase.GetCollection<ToDoItem>("Todos");
        }
        public async Task InitializeAsync()
        {
            await _mongoCollection.DeleteManyAsync(FilterDefinition<ToDoItem>.Empty);
        }

        public Task DisposeAsync() => Task.CompletedTask;


        [Fact]
        public async void Should_create_todo_item_v2()
        {
            var todoItemRequst = new ToDoItemCreateRequest()
            {
                Description = "test create",
                Done = false,
                Favorite = true,
            };

            var json = JsonSerializer.Serialize(todoItemRequst);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/v2/ToDoItemsV2", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();

            var returnedTodos = JsonSerializer.Deserialize<ToDoItemDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(returnedTodos);
            Assert.Equal("test create", returnedTodos.Description);
            Assert.True(returnedTodos.Favorite);
            Assert.False(returnedTodos.Done);
        }

        [Fact]
        public async void Should_modify_todo_item_v2()
        {

            var todoItem = new ToDoItem
            {
                Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
                Description = "Buy groceries",
                Done = false,
                Favorite = true,
                CreatedTimeDate = DateTime.Now.Date,
            };

            await _mongoCollection.InsertOneAsync(todoItem);

            //var todoItemRequst = new ToDoItemCreateRequest()
            //{
            //    Description = "modified description",
            //    Done = false,
            //    Favorite = true,
            //};

            var todoItemRequst = new ToDoItemV2Obj(
                "5f9a7d8e2d3b4a1eb8a7d8e2", // 确保这里的 ID 与 URL 中的 ID 一致
                "modified description",
                false,
                true,
                DateTime.Now.Date,
                DateTime.Now.Date,
                2,
                DateTime.Now.Date,
                DueDateRequirementType.Fewest
            );

            var json = JsonSerializer.Serialize(todoItemRequst);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/v2/ToDoItemsV2/5f9a7d8e2d3b4a1eb8a7d8e2", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

            var returnedTodos = JsonSerializer.Deserialize<ToDoItemDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(returnedTodos);
            Assert.Equal("5f9a7d8e2d3b4a1eb8a7d8e2", returnedTodos.Id);
            Assert.Equal("modified description", returnedTodos.Description);
            Assert.True(returnedTodos.Favorite);
            Assert.False(returnedTodos.Done);
        }

        //[Fact]
        //public async Task Should_modify_todo_item_v2()
        //{
        //    var todoItem = new ToDoItem
        //    {
        //        Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
        //        Description = "Buy groceries",
        //        Done = false,
        //        Favorite = true,
        //        CreatedTimeDate = DateTime.Now.Date,
        //    };

        //    await _mongoCollection.InsertOneAsync(todoItem);

        //    var todoItemRequest = new ToDoItemV2Obj(
        //        "5f9a7d8e2d3b4a1eb8a7d8e2",
        //        "test put",
        //        false,
        //        true,
        //        DateTime.Now.Date,
        //        DateTime.Now.Date,
        //        1,
        //        DateTime.Now.Date,
        //        DueDateRequirementType.Fewest
        //    );

        //    var json = JsonSerializer.Serialize(todoItemRequest);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");
        //    var response = await _client.PutAsync("/api/v2/ToDoItemsV2/5f9a7d8e2d3b4a1eb8a7d8e2", content);

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine(responseContent);

        //    var returnedTodos = JsonSerializer.Deserialize<ToDoItemV2Obj>(responseContent, new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    });

        //    Assert.NotNull(returnedTodos);
        //    Assert.Equal("5f9a7d8e2d3b4a1eb8a7d8e2", returnedTodos.Id);
        //    Assert.Equal("test put", returnedTodos.Description);
        //    Assert.True(returnedTodos.Favorite);
        //    Assert.False(returnedTodos.Done);
        //}

    }
}