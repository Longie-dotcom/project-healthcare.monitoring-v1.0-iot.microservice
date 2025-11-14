using Domain.DomainException;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entity
{
    public class DeviceProfile
    {
        #region Attributes
        #endregion

        #region Properties
        public Guid DeviceProfileID { get; private set; }
        public string IdentityNumber { get; private set; } = string.Empty; // assign from patient 
        public string FullName { get; private set; } = string.Empty;
        public DateTime Dob { get; private set; }
        public string Gender { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public DateTime? AssignedAt { get; private set; }
        public DateTime? UnassignedAt { get; private set; }

        public string EdgeKey { get; private set; }
        public string ControllerKey { get; private set; } = string.Empty;
        public string IpAddress { get; private set; } = string.Empty;
        public string BedNumber { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        
        [BsonElement("PatientSensors")]
        public List<PatientSensor> Sensors { get; private set; } = new();


        [BsonElement("PatientStaffs")]
        public List<PatientStaff> PatientStaffs { get; private set; } = new();

        // Return only active sensors that can receive data
        public IEnumerable<PatientSensor> ActiveSensors
        {
            get { return Sensors.Where(s => s.CanReceiveData); }
        }

        // Return only active staff that has access
        public IEnumerable<PatientStaff> AuthorizedStaffs
        {
            get { return PatientStaffs.Where(s => s.HasAccess); }
        }
        #endregion

        protected DeviceProfile() { }

        public DeviceProfile(
            Guid deviceProfileID,
            string identityNumber,
            string fullName,
            DateTime dob,
            string gender,
            string email,
            string phone,
            DateTime assignedAt,
            string edgeKey,
            string controllerKey,
            string ipAddress,
            string bedNumber)
        {
            DeviceProfileID = deviceProfileID;
            IdentityNumber = identityNumber;
            FullName = fullName;
            Dob = dob;
            Gender = gender;
            Email = email;
            Phone = phone;
            AssignedAt = assignedAt;

            EdgeKey = edgeKey;
            ControllerKey = controllerKey;
            IpAddress = ipAddress;
            BedNumber = bedNumber;
            IsActive = true;
        }

        #region Methods
        public void UpdateIamInfo(
            string fullName, 
            DateTime? dob, 
            string gender, 
            string email, 
            string phone)
        {
            if (!string.IsNullOrEmpty(fullName))
                FullName = fullName;

            if (dob.HasValue)
                Dob = dob.Value;

            if (!string.IsNullOrEmpty(gender))
                Gender = gender;

            if (!string.IsNullOrEmpty(email))
                Email = email;

            if (!string.IsNullOrEmpty(phone))
                Phone = phone;
        }

        public void UpdateDeviceInfo(
            string? ipAddress,
            string? bedNumber,
            bool? isActive)
        {
            if (!string.IsNullOrEmpty(ipAddress))
                IpAddress = ipAddress;

            if (!string.IsNullOrEmpty(bedNumber))
                BedNumber = bedNumber;

            if (isActive.HasValue)
                IsActive = isActive.Value;
        }

        public void AssignSensor(PatientSensor sensor)
        {
            if (sensor == null)
                throw new InvalidRoomProfileAggregateException("Sensor cannot be null.");

            var existing = Sensors.FirstOrDefault(
                s => s.SensorKey == sensor.SensorKey && s.UnassignedAt == null);
            if (existing != null)
                throw new InvalidRoomProfileAggregateException(
                    $"Sensor {sensor.SensorKey} already active.");
            Sensors.Add(sensor);
        }

        public void UnassignSensor(string sensorKey)
        {
            var s = Sensors.FirstOrDefault(
                x => x.SensorKey == sensorKey && x.UnassignedAt == null);
            if (s != null)
                s.Unassign();
        }

        public void AssignStaff(PatientStaff staff)
        {
            if (staff == null) return;

            var existing = PatientStaffs.FirstOrDefault(
                s => s.StaffIdentityNumber == staff.StaffIdentityNumber && s.UnassignedAt == null);
            if (existing != null)
                return;
            PatientStaffs.Add(staff);
        }

        public void UnassignStaff(string staffIdentityNumber)
        {
            var staff = PatientStaffs.FirstOrDefault(
                x => x.StaffIdentityNumber == staffIdentityNumber && x.UnassignedAt == null);
            if (staff != null)
                staff.Unassign();
        }

        public void ReleaseBed()
        {
            UnassignedAt = DateTime.UtcNow;
            IsActive = false;
        }
        #endregion
    }
}
