using Domain.DomainException;

namespace Domain.Entity
{
    public class Sensor
    {
        #region Properties
        public Guid SensorID { get; private set; }
        public string SensorKey { get; private set; }
        public string ControllerKey { get; private set; }
        public string Type { get; private set; }
        public string Unit { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        #endregion

        protected Sensor() { }

        public Sensor(
            Guid sensorID, 
            string sensorKey,
            string controllerKey,
            string type, 
            string unit, 
            string description)
        {
            ValidateSensorID(sensorID);
            ValidateSensorKey(sensorKey);
            ValidateControllerKey(controllerKey);

            SensorID = sensorID;
            SensorKey = sensorKey;
            ControllerKey = controllerKey;
            Type = type;
            Unit = unit;
            Description = description;
            IsActive = true;
        }

        #region Methods
        public void UpdateType(string type)
        {
            Type = type;
        }

        public void UpdateUnit(string unit)
        {
            Unit = unit;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }

        public void UpdateActive(bool isActive)
        {
            IsActive = isActive;
        }
        #endregion

        #region Validators
        private void ValidateSensorID(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEdgeDeviceAggregateException(
                    "SensorID cannot be empty.");
        }

        private void ValidateSensorKey(string sensorKey)
        {
            if (string.IsNullOrWhiteSpace(sensorKey))
                throw new InvalidEdgeDeviceAggregateException(
                    "SensorKey cannot be empty.");
        }

        private void ValidateControllerKey(string controllerKey)
        {
            if (string.IsNullOrWhiteSpace(controllerKey))
                throw new InvalidEdgeDeviceAggregateException(
                    "ControllerKey cannot be empty.");
        }
        #endregion
    }
}
