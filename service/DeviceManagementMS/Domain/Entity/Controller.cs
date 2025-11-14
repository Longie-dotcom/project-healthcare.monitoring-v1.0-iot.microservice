using Domain.DomainException;
using Domain.ValueObject;

namespace Domain.Entity
{
    public class Controller
    {
        #region Attributes
        private readonly List<Sensor> sensors = new();
        #endregion

        #region Properties
        public Guid ControllerID { get; private set; }
        public string ControllerKey { get; private set; }
        public string EdgeKey { get; private set; }
        public string BedNumber { get; private set; }
        public string IpAddress { get; private set; }
        public string FirmwareVersion { get; private set; }
        public string Status { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<Sensor> Sensors
        {
            get { return sensors.AsReadOnly(); }
        }
        #endregion

        protected Controller() { }

        public Controller(
            Guid controllerID, 
            string controllerKey, 
            string edgeKey, 
            string bedNumber, 
            string ipAddress,
            string firmwareVersion)
        {
            ValidateControllerID(controllerID);
            ValidateControllerKey(controllerKey);
            ValidateEdgeKey(edgeKey);
            ValidateIpAddress(ipAddress);

            ControllerID = controllerID;
            ControllerKey = controllerKey;
            EdgeKey = edgeKey;
            BedNumber = bedNumber;
            IpAddress = ipAddress;
            FirmwareVersion = firmwareVersion;
            Status = ControllerStatusMemo.Online.Name;
            IsActive = true;
        }

        #region Methods
        public void UpdateBedNumber(string bedNumber)
        {
            BedNumber = bedNumber;
        }

        public void UpdateIpAddress(string ipAddress)
        {
            ValidateIpAddress(ipAddress);
            IpAddress = ipAddress;
        }

        public void UpdateFirmwareVersion(string firmwareVersion)
        {
            FirmwareVersion = firmwareVersion;
        }

        public void UpdateStatus(string status)
        {
            var existed = ControllerStatusMemo.GetByName(status);
            if (existed == null)
                throw new InvalidEdgeDeviceAggregateException(
                    "Status must be Online, Offline, or Maintenance.");
            Status = status;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;

            foreach (var sensor in sensors)
            {
                sensor.UpdateActive(false);
            }
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;

            foreach (var sensor in sensors)
            {
                sensor.UpdateActive(true);
            }
        }


        public void AddSensor(Sensor sensor)
        {
            if (sensor == null)
                throw new InvalidEdgeDeviceAggregateException(
                    "Sensor cannot be null.");

            if (!sensor.ControllerKey.Equals(ControllerKey, StringComparison.OrdinalIgnoreCase))
                throw new InvalidEdgeDeviceAggregateException(
                    "Sensor does not belong to this Controller.");

            var existingSensor = sensors.FirstOrDefault(s => s.SensorKey == sensor.SensorKey);

            if (existingSensor != null)
            {
                // Already active → throw error
                throw new InvalidEdgeDeviceAggregateException(
                    $"SensorKey '{sensor.SensorKey}' already exists and is active in this Controller.");
            }

            sensors.Add(sensor);
        }
        #endregion

        #region Validators
        private void ValidateControllerID(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEdgeDeviceAggregateException(
                    "ControllerID cannot be empty.");
        }

        private void ValidateControllerKey(string controllerKey)
        {
            if (string.IsNullOrWhiteSpace(controllerKey))
                throw new InvalidEdgeDeviceAggregateException(
                    "ControllerKey cannot be empty.");
        }

        private void ValidateEdgeKey(string edgeKey)
        {
            if (string.IsNullOrWhiteSpace(edgeKey))
                throw new InvalidEdgeDeviceAggregateException(
                    "EdgeKey cannot be empty.");
        }

        private void ValidateIpAddress(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new InvalidEdgeDeviceAggregateException(
                    "IpAddress cannot be empty.");
        }
        #endregion
    }
}
