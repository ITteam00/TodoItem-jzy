using ToDoList.Api.Models;

namespace ToDoList.Api.Services
{
    public interface IToDoItemsService
    {
        Task CreateAsync(ToDoItemDto toDoItemDto);
        Task<ToDoItemDto?> GetAsync(string id);
        Task<List<ToDoItemDto>> GetAllAsync();
        Task<bool> RemoveAsync(string id);
        Task ReplaceAsync(string id, ToDoItemDto updatedToDoItemDto);
    }
}
