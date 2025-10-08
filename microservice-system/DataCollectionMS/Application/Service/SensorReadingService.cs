using Domain.Aggregate;
using Domain.IRepository;

namespace Application.Service
{
    public class SensorReadingService
    {
        #region Attributes
        private readonly ISensorReadingRepository _repository;
        #endregion

        #region Properties
        #endregion

        public SensorReadingService(ISensorReadingRepository repository)
        {
            _repository = repository;   
        }

        #region Methods
        public async Task ProcessReadingAsync(SensorReading reading)
        {
            await _repository.SaveAsync(reading);
        }
        #endregion
    }
}
