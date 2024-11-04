using ToDoList.Api.Models;

namespace ToDoList.Api.Services
{
    public interface IToDoItemsV2Service
    {

        Task<ToDoItemV2Obj> CreateToDoItem(ToDoItemV2Obj inputToDoItem);
        Task<ToDoItemV2Obj> EditToDoItem(ToDoItemV2Obj inputToDoItem);

    }
}
