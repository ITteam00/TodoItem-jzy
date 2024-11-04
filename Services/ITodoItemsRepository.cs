using MongoDB.Driver;
using ToDoList.Api.Models;

namespace ToDoList.Api.Services
{
    public interface ITodoItemsRepository
    {

        public Task<List<ToDoItemV2Obj>> FindAllTodoItemsInOneDay(DateTime dateTime);
        public Task<ToDoItemV2Obj> FindById(string id);
        public Task<UpdateResult> Save(ToDoItemV2Obj todoItem);
        public Task<ToDoItemV2Obj> CreateAsync(ToDoItemV2Obj toDoItem);
        public Task<ToDoItemV2Obj> EditItem(ToDoItemV2Obj item);
    }
}
