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
        [BsonId]
        [BsonRepresentation(BsonType.String)]
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

        public DeviceProfile() { }

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
        public IEnumerable<PatientStaff> GetActiveStaffs()
        {
            var staffs = PatientStaffs.FindAll(
                s => s.UnassignedAt == null);
            return staffs;
        }

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

            foreach (var s in Sensors.Where(s => s.UnassignedAt == null))
                s.Unassign();

            foreach (var st in PatientStaffs.Where(st => st.UnassignedAt == null))
                st.Unassign();
        }



        /// <summary>
        /// Add a single sensor reading by matching the second segment of SensorKey with sensorName.
        /// </summary>
        public (PatientSensor Sensor, string DataType, object DataValue)? AddSensorDataByName(
            string sensorName, string dataType, object dataValue)
        {
            var sensor = Sensors.FirstOrDefault(s =>
                s.CanReceiveData &&
                s.SensorKey.Contains(':') &&
                s.SensorKey.Split(':').Length > 1 &&
                string.Equals(s.SensorKey.Split(':')[1].Split('-')[0], sensorName, StringComparison.OrdinalIgnoreCase)
            );

            if (sensor == null)
                return null;

            // Handle JsonElement coming from System.Text.Json deserialization
            object rawValue = dataValue;
            if (dataValue is System.Text.Json.JsonElement je)
            {
                rawValue = je.ValueKind switch
                {
                    System.Text.Json.JsonValueKind.Number when dataType.ToLower() == "int" => je.GetInt32(),
                    System.Text.Json.JsonValueKind.Number when dataType.ToLower() == "double" => je.GetDouble(),
                    System.Text.Json.JsonValueKind.String => je.GetString() ?? "",
                    System.Text.Json.JsonValueKind.True => true,
                    System.Text.Json.JsonValueKind.False => false,
                    _ => je.GetRawText()
                };
            }

            // Convert to BsonValue (MongoDB compatible)
            BsonValue bsonValue = dataType.ToLower() switch
            {
                "int" => new BsonInt32(Convert.ToInt32(rawValue)),
                "double" => new BsonDouble(Convert.ToDouble(rawValue)),
                "string" => new BsonString(Convert.ToString(rawValue)),
                _ => new BsonString(Convert.ToString(rawValue))
            };

            sensor.AddData(dataType, bsonValue);
            return (sensor, dataType, dataValue);
        }

        /// <summary>
        /// Add multiple sensor readings at once
        /// </summary>
        public IEnumerable<(PatientSensor Sensor, string DataType, object DataValue)> AddMultipleSensorDataByName(
            IEnumerable<(string sensorName, string dataType, object dataValue)> sensorValues)
        {
            var listSensor = new List<(PatientSensor Sensor, string DataType, object DataValue)>();

            foreach (var sv in sensorValues)
            {
                var sensor = AddSensorDataByName(sv.sensorName, sv.dataType, sv.dataValue);
                if (sensor != null)
                    listSensor.Add(sensor.Value);
            }

            return listSensor;
        }
        #endregion
    }
}
