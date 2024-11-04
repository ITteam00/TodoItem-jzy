using ToDoList.Api.Models;
using ToDoList.Api.Services;

namespace TodoItems.Core
{
    public class TodoItemV2Service : IToDoItemsV2Service
    {
        private const int MAX_EDIT_TIMES = 2;
        private const int MAX_DUEDATE = 8;
        private readonly ITodoItemsRepository _todosRepository;

        public TodoItemV2Service(ITodoItemsRepository repository)
        {
            _todosRepository = repository;
        }
        public async Task<ToDoItemV2Obj> CreateToDoItem(ToDoItemV2Obj inputToDoItem)
        {

            return await _todosRepository.CreateAsync(inputToDoItem);
        }

        public async Task<ToDoItemV2Obj> EditToDoItem(ToDoItemV2Obj inputToDoItem)
        {
            String id = inputToDoItem.Id;
            var existingItem = await _todosRepository.FindById(id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException("Todo item not found.");
            }

            existingItem.Description = inputToDoItem.Description;
            existingItem.Favorite = inputToDoItem.Favorite;
            existingItem.DueDate = inputToDoItem.DueDate;

            return await _todosRepository.EditItem(existingItem);
        }

        public async Task<ToDoItemV2Obj> GetToDoItemById(string id)
        {
            return await _todosRepository.FindById(id);
        }

        public async Task<List<ToDoItemV2Obj>> GetAllToDoItemsInOneDay(DateTime date)
        {
            return await _todosRepository.FindAllTodoItemsInOneDay(date);
        }


        Task<ToDoItemV2Obj> IToDoItemsV2Service.CreateAsync(ToDoItemV2Obj inputToDoItem)
        {
            throw new NotImplementedException();
        }

        Task IToDoItemsV2Service.FindById(string id)
        {
            throw new NotImplementedException();
        }

        Task<ToDoItemV2Obj> IToDoItemsV2Service.EditItem(object existingItem)
        {
            throw new NotImplementedException();
        }
    }
}
