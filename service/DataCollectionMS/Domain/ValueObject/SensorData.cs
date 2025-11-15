using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.ValueObject
{
    public class SensorData
    {
        #region Attributes
        #endregion

        #region Properties
        public BsonValue Value { get; private set; }
        public DateTime RecordedAt { get; private set; }
        #endregion

        protected SensorData() { }

        public SensorData(
            BsonValue value)
        {
            Value = value;
            RecordedAt = DateTime.UtcNow;
        }

        #region Methods
        #endregion
    }
}
