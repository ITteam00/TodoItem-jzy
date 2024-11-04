using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToDoList.Api.Models
{
    [BsonIgnoreExtraElements]
    public record ToDoItem
    {
        [BsonId]
        public string Id { get; init; }
        public string Description { get; set; }
        public bool Done { get; set; }
        public bool Favorite { get; set; }

        public DateTime CreatedTimeDate { get; init; }
        public DateTime LastModifiedTimeDate { get; set; }
        public int EditTimes { get; set; }
        public DateTime DueDate { get; set; }


    }
}
