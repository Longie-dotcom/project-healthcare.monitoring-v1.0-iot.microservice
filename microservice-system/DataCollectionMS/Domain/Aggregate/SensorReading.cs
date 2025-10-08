using Domain.Entity;
using Domain.Enum;

namespace Domain.Aggregate
{
    public class SensorReading
    {
        public Guid SensorReadingId { get; private set; }
        public Guid DeviceId { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public List<DataReading> Readings { get; private set; }
        public ValidationStatus Status { get; private set; }

        protected SensorReading() { }

        public SensorReading(
            Guid deviceId, 
            List<DataReading> readings, 
            ValidationStatus status)
        {
            SensorReadingId = Guid.NewGuid();
            DeviceId = deviceId;
            Readings = readings;
            TimeStamp = DateTime.UtcNow;
            Status = status;
        }
    }
}
