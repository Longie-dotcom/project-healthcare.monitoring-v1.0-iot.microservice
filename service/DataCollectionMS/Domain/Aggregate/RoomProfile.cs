using Domain.DomainException;
using Domain.Entity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Aggregate
{
    public class RoomProfile : AggregateBase
    {
        #region Attributes
        #endregion

        #region Properties
        public string EdgeKey { get; private set; } = string.Empty;
        public string IpAddress { get; private set; } = string.Empty;
        public string RoomName { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        [BsonElement("DeviceProfiles")]
        public List<DeviceProfile> DeviceProfiles { get; private set; } = new();
        #endregion

        public RoomProfile() { }

        public RoomProfile(
            Guid roomProfileID,
            string edgeKey,
            string ipAddress,
            string roomName)
        {
            base.ID = roomProfileID;
            EdgeKey = edgeKey;
            IpAddress = ipAddress;
            RoomName = roomName;
            IsActive = true;
        }

        #region Methods
        public DeviceProfile GetAssignedDeviceProfileByControllerKey(string controllerKey)
        {
            var device = DeviceProfiles.FirstOrDefault(
                d => d.ControllerKey == controllerKey && d.UnassignedAt == null);
            if (device == null)
                throw new InvalidRoomProfileAggregateException(
                    $"Active device with controller key '{controllerKey}' not found in room '{EdgeKey}'.");
            return device;
        }

        public DeviceProfile GetAssignedDeviceProfileByIP(string controllerIP)
        {
            var device = DeviceProfiles.FirstOrDefault(
                d => d.IpAddress == controllerIP && d.UnassignedAt == null);
            if (device == null)
                throw new InvalidRoomProfileAggregateException(
                    $"Active device with controller ip '{controllerIP}' not found in room '{EdgeKey}'.");
            return device;
        }

        public DeviceProfile GetAssignedDeviceProfileByPatientIdentity(string identityNumber)
        {
            var device = DeviceProfiles.FirstOrDefault(
                d => d.IdentityNumber == identityNumber && d.UnassignedAt == null);
            if (device == null)
                throw new InvalidRoomProfileAggregateException(
                    $"Active device of patient: '{identityNumber}' not found in room '{EdgeKey}'.");
            return device;
        }

        public PatientSensor GetAssignedSensor(string controllerKey, string sensorKey)
        {
            var device = GetAssignedDeviceProfileByControllerKey(controllerKey);
            var sensor = device.Sensors.FirstOrDefault(
                s => s.SensorKey == sensorKey && s.UnassignedAt == null);
            if (sensor == null)
                throw new InvalidRoomProfileAggregateException(
                    $"Active sensor '{sensorKey}' not found on controller '{controllerKey}'.");
            return sensor;
        }

        public PatientStaff GetActiveStaff(string staffIdentityNumber)
        {
            var staff = DeviceProfiles.SelectMany(d => d.PatientStaffs).FirstOrDefault(
                s => s.StaffIdentityNumber == staffIdentityNumber && s.UnassignedAt == null);
            if (staff == null)
                throw new InvalidRoomProfileAggregateException(
                    $"Active staff '{staffIdentityNumber}' not found in room '{EdgeKey}'.");
            return staff;
        }

        public void UpdateInfo(
            string? ipAddress,
            string? roomName,
            bool? isActive)
        {
            if (!string.IsNullOrEmpty(ipAddress))
                IpAddress = ipAddress;

            if (!string.IsNullOrEmpty(roomName))
                RoomName = roomName;

            if (isActive.HasValue)
                IsActive = isActive.Value;
        }

        public void AssignBed(DeviceProfile deviceProfile)
        {
            if (deviceProfile == null)
                throw new InvalidRoomProfileAggregateException(
                    "Device profile cannot be null.");

            // Check controller key conflict
            var existing = DeviceProfiles.FirstOrDefault(
                df => df.ControllerKey == deviceProfile.ControllerKey && df.UnassignedAt == null);
            if (existing != null)
                throw new InvalidRoomProfileAggregateException(
                    $"Device controller at bed: {existing.BedNumber} is assigned.");

            // Check bed number conflict
            var bedConflict = DeviceProfiles.FirstOrDefault(
                df => df.BedNumber == deviceProfile.BedNumber && df.UnassignedAt == null);
            if (bedConflict != null)
                throw new InvalidRoomProfileAggregateException(
                    $"Bed '{deviceProfile.BedNumber}' is already assigned to another active device.");

            DeviceProfiles.Add(deviceProfile);
        }

        public void ReleaseBed(string controllerKey)
        {
            var device = DeviceProfiles.FirstOrDefault(
                x => x.ControllerKey == controllerKey && x.UnassignedAt == null);
            if (device == null)
                throw new InvalidRoomProfileAggregateException(
                    $"No active device found to release for controller '{controllerKey}'.");
            device.ReleaseBed();
        }
        #endregion
    }
}
