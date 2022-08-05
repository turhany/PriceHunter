using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PriceHunter.Data.MongoDB.Entity
{
    public interface IMongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }
    }
}
