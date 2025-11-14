using Domain.DomainException;
using Domain.Entity;

namespace Domain.Aggregate
{
    public class EdgeDevice
    {
        #region Attributes
        private readonly List<Controller> controllers = new();
        #endregion

        #region Properties
        public Guid EdgeDeviceID { get; private set; }
        public string EdgeKey { get; private set; }
        public string RoomName { get; private set; }
        public string IpAddress { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<Controller> Controllers
        {
            get { return controllers.AsReadOnly(); }
        }
        #endregion

        #region Constructors
        protected EdgeDevice() { }

        public EdgeDevice(
            Guid edgeDeviceID, 
            string edgeKey, 
            string roomName, 
            string ipAddress, 
            string description)
        {
            ValidateEdgeDeviceID(edgeDeviceID);
            ValidateEdgeKey(edgeKey);
            ValidateIpAddress(ipAddress);

            EdgeDeviceID = edgeDeviceID;
            EdgeKey = edgeKey;
            RoomName = roomName;
            IpAddress = ipAddress;
            Description = description;
            IsActive = true;
        }
        #endregion

        #region Methods
        public void UpdateRoomName(string roomName)
        {
            RoomName = roomName;
        }

        public void UpdateIpAddress(string ipAddress)
        {
            ValidateIpAddress(ipAddress);
            IpAddress = ipAddress;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;

            foreach (var controller in controllers)
            {
                controller.Deactivate();
            }
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;

            foreach (var controller in controllers)
            {
                controller.Activate();
            }
        }


        public void AddController(Controller controller)
        {
            if (controller == null)
                throw new InvalidEdgeDeviceAggregateException(
                    "Controller cannot be null.");

            if (!controller.EdgeKey.Equals(EdgeKey, StringComparison.OrdinalIgnoreCase))
                throw new InvalidEdgeDeviceAggregateException(
                    "Controller does not belong to this EdgeDevice.");

            var existingController = controllers.FirstOrDefault(c => c.ControllerKey == controller.ControllerKey);

            if (existingController != null)
            {
                throw new InvalidEdgeDeviceAggregateException(
                    $"ControllerKey '{controller.ControllerKey}' already exists and is active in this EdgeDevice.");
            }

            controllers.Add(controller);
        }
        #endregion

        #region Validators
        private void ValidateEdgeDeviceID(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidEdgeDeviceAggregateException(
                    "EdgeDeviceID cannot be empty.");
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
