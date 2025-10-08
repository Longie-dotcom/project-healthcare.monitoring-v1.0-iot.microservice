using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface ISensorReadingRepository
    {
        Task<SensorReading?> GetByIdAsync(Guid sensorReadingId);
        Task SaveAsync(SensorReading sensorReading);
        Task<List<SensorReading>> GetByDeviceAsync(
            Guid deviceId, 
            DateTime? from = null, 
            DateTime? to = null);
    }
}
