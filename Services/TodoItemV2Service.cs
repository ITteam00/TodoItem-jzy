using ToDoList.Api.Models;

namespace ToDoList.Api.Services
{
    public class TodoItemV2Service
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


    }
}
