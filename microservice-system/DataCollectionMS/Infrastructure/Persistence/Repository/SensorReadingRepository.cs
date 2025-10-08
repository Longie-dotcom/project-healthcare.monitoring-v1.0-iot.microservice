using Domain.Aggregate;
using Domain.IRepository;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repository
{
    public class SensorReadingRepository : ISensorReadingRepository
    {
        #region Attributes
        private readonly IMongoCollection<SensorReading> _collection;
        #endregion

        #region Properties
        #endregion

        public SensorReadingRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<SensorReading>("SensorReadings");
        }

        #region Methods
        public async Task<SensorReading?> GetByIdAsync(Guid sensorReadingId)
        {
            return await _collection
                .Find(r => r.SensorReadingId == sensorReadingId)
                .FirstOrDefaultAsync();
        }

        public async Task SaveAsync(SensorReading sensorReading)
        {
            await _collection.InsertOneAsync(sensorReading);
        }

        public async Task<List<SensorReading>> GetByDeviceAsync(
            Guid deviceId,
            DateTime? from = null,
            DateTime? to = null)
        {
            var filterBuilder = Builders<SensorReading>.Filter;
            var filter = filterBuilder.Eq(r => r.DeviceId, deviceId);

            if (from.HasValue)
            {
                filter &= filterBuilder.Gte(r => r.TimeStamp, from.Value);
            }

            if (to.HasValue)
            {
                filter &= filterBuilder.Lte(r => r.TimeStamp, to.Value);
            }

            return await _collection.Find(filter).ToListAsync();
        }
        #endregion
    }
}
