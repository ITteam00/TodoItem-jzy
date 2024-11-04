using ToDoList.Api.Models;

namespace ToDoList.Api.Services
{
    public class InMemoryToDoItemsService : IToDoItemsService
    {
        private static readonly List<ToDoItemDto> _toDoItemDtos = new List<ToDoItemDto>();

        public Task<List<ToDoItemDto>> GetAllAsync()
        {
            return Task.FromResult(_toDoItemDtos);
        }

        public Task<ToDoItemDto?> GetAsync(string id)
        {
            var result = _toDoItemDtos.Find(x => x.Id == id);
            return Task.FromResult(result);
        }

        public Task CreateAsync(ToDoItemDto toDoItemDto)
        {
            _toDoItemDtos.Add(toDoItemDto);
            return Task.CompletedTask;
        }

        public Task<bool> RemoveAsync(string id)
        {
            var itemToRemove = _toDoItemDtos.Find(x => x.Id == id);
            if (itemToRemove != null)
            {
                _toDoItemDtos.Remove(itemToRemove);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task ReplaceAsync(string id, ToDoItemDto updatedToDoItemDto)
        {
            var index = _toDoItemDtos.FindIndex(x => x.Id == id);
            if (index >= 0)
            {
                updatedToDoItemDto.CreatedTime = _toDoItemDtos[index].CreatedTime;
                _toDoItemDtos[index] = updatedToDoItemDto;
            }
            return Task.CompletedTask;
        }
    }
}
