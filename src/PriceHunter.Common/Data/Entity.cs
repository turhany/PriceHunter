using MongoDB.Bson.Serialization.Attributes;

namespace PriceHunter.Common.Data
{
    public class Entity
    {
        [BsonId]
        public Guid Id { get; set; }

        public Guid? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public Guid? UpdatedBy { get; set; } 
    }
}