using Domain.ValueObject;

namespace Domain.Entity
{
    public class DataReading
    {
        public Guid DataReadingID { get; private set; }
        public Measurement Measurement { get; private set; }

        protected DataReading() { }

        public DataReading(Guid dataReadingID, Measurement measurement)
        {
            DataReadingID = dataReadingID;
            Measurement = measurement;
        }
    }
}
