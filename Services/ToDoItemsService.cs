using ToDoList.Api.Models;
using Microsoft.Extensions.Options;

using MongoDB.Driver;


namespace ToDoList.Api.Services
{
    public class ToDoItemsService : IToDoItemsService
    {
        private readonly IMongoCollection<ToDoItem> _ToDoItemsCollection;

        public ToDoItemsService(
            IOptions<ToDoItemDatabaseSettings> ToDoItemStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                ToDoItemStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                ToDoItemStoreDatabaseSettings.Value.DatabaseName);

            _ToDoItemsCollection = mongoDatabase.GetCollection<ToDoItem>(
                ToDoItemStoreDatabaseSettings.Value.CollectionName);
        }


        public async Task CreateAsync(ToDoItemDto toDoItemDto)
        {
            if (toDoItemDto == null)
            {
                throw new ArgumentNullException(nameof(toDoItemDto));
            }

            ToDoItem existingItem = await _ToDoItemsCollection
                .Find(x => x.Id == toDoItemDto.Id || x.Description == toDoItemDto.Description)
                .FirstOrDefaultAsync();

            if (existingItem != null)
            {
                throw new Exception("ToDoItemDto already exist!");
            }

            var toDoItem = new ToDoItem
            {
                Id = Guid.NewGuid().ToString(),
                Description = toDoItemDto.Description,
                Favorite = toDoItemDto.Favorite,
                Done = toDoItemDto.Done,
                CreatedTime = toDoItemDto.CreatedTime
            };

            await _ToDoItemsCollection.InsertOneAsync(toDoItem);

            return;
        }



        public async Task<List<ToDoItemDto>> GetAllAsync()
        {
            List<ToDoItem> items = await _ToDoItemsCollection.Find(_ => true).ToListAsync();
            List<ToDoItemDto> dtoList = new List<ToDoItemDto>();
            foreach (var item in items)
            {
                ToDoItemDto itemDto = new ToDoItemDto
                {
                    Id = item.Id,
                    Description = item.Description,
                    Favorite = item.Favorite,
                    Done = item.Done,
                    CreatedTime = item.CreatedTime
                };
                dtoList.Add(itemDto);
            }

            return dtoList;
        }

        public async Task<ToDoItemDto?> GetAsync(string id)
        {
            if (_ToDoItemsCollection == null) {
                return null;
            }
            ToDoItem? item = await _ToDoItemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (item == null) { return null; }
            ToDoItemDto itemDto = new ToDoItemDto
            {
                Id = item.Id,
                Description = item.Description,
                Favorite = item.Favorite,
                Done = item.Done,
                CreatedTime = item.CreatedTime
            };

            return itemDto;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            ToDoItem? item = await _ToDoItemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (item != null)
            {
                await _ToDoItemsCollection.DeleteOneAsync(x => x.Id == id);
                return true;
            }
            return false;
        }

        public async Task ReplaceAsync(string id, ToDoItemDto updatedToDoItemDto)
        {
            ToDoItem? item = await _ToDoItemsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (item != null)
            {
                var updatedToDoItem = new ToDoItem
                {
                    Id = id, // 保持ID不变
                    Description = updatedToDoItemDto.Description,
                    Favorite = updatedToDoItemDto.Favorite,
                    Done = updatedToDoItemDto.Done,
                    CreatedTime = updatedToDoItemDto.CreatedTime
                };
                var result = await _ToDoItemsCollection.ReplaceOneAsync(x => x.Id == id, updatedToDoItem);
            }
            else
            {
                throw new NullReferenceException();

            }

        }
    }
}
