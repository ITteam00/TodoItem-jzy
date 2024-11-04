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
            _factory = factory;
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

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

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

            var todoItemRequst = new ToDoItemCreateRequest()
            {
                Description = "test put",
                Done = false,
                Favorite = true,
            };

            var json = JsonSerializer.Serialize(todoItemRequst);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/v2/ToDoItemsV2/5f9a7d8e2d3b4a1eb8a7d8e2", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();

            var returnedTodos = JsonSerializer.Deserialize<ToDoItemDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(returnedTodos);
            Assert.Equal("test put", returnedTodos.Description);
            Assert.True(returnedTodos.Favorite);
            Assert.False(returnedTodos.Done);
        }

    }
}