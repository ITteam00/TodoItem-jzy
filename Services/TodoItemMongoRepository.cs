using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToDoList.Api.Models;
using ToDoList.Api.Services;

namespace TodoItem.Infrastructure;

public class TodoItemMongoRepository : ITodoItemsRepository
{
    private readonly IMongoCollection<ToDoItem?> _todosCollection;
    private const int MAX_DUEDATE = 8;
    private const int MAX_DUE_DATE_IN_ONE_DAY = 8;
    private const int MAX_DUE_DATE_RANGE = 5;
    private readonly DueDateStrategy _dueDateStrategy;


    public TodoItemMongoRepository(IOptions<TodoStoreDatabaseSettings> todoStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(todoStoreDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(todoStoreDatabaseSettings.Value.DatabaseName);
        _todosCollection = mongoDatabase.GetCollection<ToDoItem>(todoStoreDatabaseSettings.Value.CollectionName);
        _dueDateStrategy = new DueDateStrategy(this);
    }

    public async Task<ToDoItemV2Obj> FindById(string id)
    {
        FilterDefinition<ToDoItem?> filter = Builders<ToDoItem>.Filter.Eq(x => x.Id, id);
        ToDoItem? todoItemDao = await _todosCollection.Find(filter).FirstOrDefaultAsync();

        ToDoItemV2Obj todoItem = ConvertToTodoItem(todoItemDao);
        return todoItem;
    }

    private ToDoItemV2Obj ConvertToTodoItem(ToDoItem? todoItemDao)
    {
        if (todoItemDao == null) return null;

        return new ToDoItemV2Obj(
            todoItemDao.Id,
            todoItemDao.Description,
            todoItemDao.Done,
            todoItemDao.Favorite,
            todoItemDao.CreatedTimeDate,
            todoItemDao.LastModifiedTimeDate,
            todoItemDao.EditTimes,
            todoItemDao.DueDate,
            DueDateRequirementType.Earliest
        );
    }

    public async Task<UpdateResult> Save(ToDoItemV2Obj todoItem)
    {
        var filter = Builders<ToDoItem>.Filter.Eq(x => x.Id, todoItem.Id);
        var update = Builders<ToDoItem>.Update
            .Set(x => x.Description, todoItem.Description)
            .Set(x => x.Done, todoItem.Done)
            .Set(x => x.Favorite, todoItem.Favorite)
            .Set(x => x.CreatedTimeDate, todoItem.CreatedTimeDate)
            .Set(x => x.LastModifiedTimeDate, todoItem.LastModifiedTimeDate)
            .Set(x => x.EditTimes, todoItem.EditTimes)
            .Set(x => x.DueDate, todoItem.DueDate);

        return await _todosCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task<List<ToDoItemV2Obj>> FindAllTodoItemsInOneDay(DateTime dateTime)
    {
        var filter = Builders<ToDoItem>.Filter.Gte(x => x.DueDate, dateTime.Date) &
                     Builders<ToDoItem>.Filter.Lt(x => x.DueDate, dateTime.Date.AddDays(1));
        var todoItemsDao = await _todosCollection.Find(filter).ToListAsync();

        return todoItemsDao.Select(ConvertToTodoItem).ToList();
    }

    public async Task<ToDoItemV2Obj> CreateAsync(ToDoItemV2Obj inputToDoItem)
    {

        if (inputToDoItem.DueDate == null && inputToDoItem.DueDateRequirement == null)
        {
            throw new InvalidOperationException("Due Date and DueDateRequirement cannot be empty at the same time.");
        }

        inputToDoItem.DueDate = await _dueDateStrategy.DetermineDueDateAsync(inputToDoItem);

        await Save(inputToDoItem);
        return await FindById(inputToDoItem.Id);

    }

    public async Task<ToDoItemV2Obj> EditItem(ToDoItemV2Obj item)
    {
        DateTime lastModifiedDate = item.LastModifiedTimeDate;
        DateTime currentDate = DateTimeOffset.Now.Date;
        TimeSpan difference = currentDate - lastModifiedDate;

        if (difference.Days >= 1)
        {
            item.EditTimes = 1;
        }
        else
        {
            item.IncrementEditTimes();
        }

        item.LastModifiedTimeDate = currentDate;
        await Save(item);
        return item;
    }

}