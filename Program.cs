using Microsoft.AspNetCore.Mvc;
using TodoItem.Infrastructure;
using TodoItems.Core;
using ToDoList.Api;
using ToDoList.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IToDoItemsService, ToDoItemsService>();
builder.Services.AddSingleton<IToDoItemsV2Service, TodoItemV2Service>();
builder.Services.Configure<ToDoItemDatabaseSettings>(builder.Configuration.GetSection("ToDoItemDatabase"));


builder.Services.Configure<TodoStoreDatabaseSettings>(builder.Configuration.GetSection("ToDoItemDatabase"));
builder.Services.AddSingleton<ITodoItemsRepository, TodoItemMongoRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.Run();

public partial class Program { }