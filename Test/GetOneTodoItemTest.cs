using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MongoDB.Driver;
using System.Net;
using System.Text.Json;
using ToDoList.Api.Models;
using ToDoList.Api.Services;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using ikvm.runtime;

namespace ToDoList.Api.ApiTests
{
    public class ToDoItemsV2ControllerTests
    {
        private readonly HttpClient _client;
        private readonly IMongoCollection<ToDoItem> _mongoCollection;

        public ToDoItemsV2ControllerTests()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IMongoClient, MongoClient>(sp =>
                    {
                        return new MongoClient("mongodb://localhost:27017");
                    });

                    services.AddScoped(sp =>
                    {
                        var client = sp.GetRequiredService<IMongoClient>();
                        var database = client.GetDatabase("TodoTestStore");
                        return database.GetCollection<ToDoItem>("Todos");
                    });

                    services.AddScoped<IToDoItemsService, ToDoItemsService>();
                });

            var server = new TestServer(builder);
            _client = server.CreateClient();

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var mongoDatabase = mongoClient.GetDatabase("TodoTestStore");
            _mongoCollection = mongoDatabase.GetCollection<ToDoItem>("Todos");
        }

        [Fact]
        public async Task Should_Get_Todo_By_Given_Id()
        {
            // Arrange
            var todoItem = new ToDoItem
            {
                Id = "5f9a7d8e2d3b4a1eb8a7d8e2",
                Description = "Buy groceries",
                Done = false,
                Favorite = true,
                CreatedTime = DateTime.UtcNow.Date,
            };

            await _mongoCollection.InsertOneAsync(todoItem);

            // Act
            var response = await _client.GetAsync("/api/v2/todoitemsv2/5f9a7d8e2d3b4a1eb8a7d8e2");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();

            var returnedTodo = JsonSerializer.Deserialize<ToDoItemDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(returnedTodo);
            Assert.Equal("Buy groceries", returnedTodo.Description);
            Assert.True(returnedTodo.Favorite);
            Assert.False(returnedTodo.Done);
        }

        [Fact]
        public async Task Should_Get_NotFound_By_Invalid_Id()
        {
            // Act
            var response = await _client.GetAsync("/api/v2/todoitemsv2/not_exist_id");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
