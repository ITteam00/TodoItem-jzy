using ToDoList.Api.Models;

namespace ToDoList.Api.Services
{
    public interface IToDoItemsV2Service
    {

        Task<ToDoItemV2Obj> CreateToDoItem(ToDoItemV2Obj inputToDoItem);
        Task<ToDoItemV2Obj> EditToDoItem(ToDoItemV2Obj inputToDoItem);
        Task<ToDoItemV2Obj> GetToDoItemById(string id);
        Task<List<ToDoItemV2Obj>> GetAllToDoItemsInOneDay(DateTime date);
        Task<ToDoItemV2Obj> CreateAsync(ToDoItemV2Obj inputToDoItem);
        Task FindById(string id);
        Task<ToDoItemV2Obj> EditItem(object existingItem);
    }
}
