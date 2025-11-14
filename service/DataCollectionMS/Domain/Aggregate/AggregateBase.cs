using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Aggregate
{
    public class AggregateBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid ID { get; protected set; }
    }
}
