using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToDoList.Api.Models
{
    [BsonIgnoreExtraElements]
    public record ToDoItem
    {
        [BsonId]
        public required string Id { get; init; }
        public required string Description { get; set; }
        public required bool Done { get; set; }
        public required bool Favorite { get; set; }

        [BsonRepresentation(BsonType.String)]
        public required DateTimeOffset CreatedTime { get; init; }

        public static implicit operator Task<object>(ToDoItem v)
        {
            throw new NotImplementedException();
        }
    }
}
