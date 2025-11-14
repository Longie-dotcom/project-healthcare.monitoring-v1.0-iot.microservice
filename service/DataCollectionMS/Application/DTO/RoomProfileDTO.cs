using MongoDB.Bson;

namespace Application.DTO
{
    public class RoomProfileDTO
    {
        public string EdgeKey { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<DeviceProfileDTO> DeviceProfiles { get; set; } = new();
    }

    public class DeviceProfileDTO
    {
        public Guid DeviceProfileID { get; set; }
        public string IdentityNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? AssignedAt { get; set; }
        public DateTime? UnassignedAt { get; set; }

        public string ControllerKey { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<PatientSensorDTO> Sensors { get; set; } = new();
        public List<PatientStaffDTO> PatientStaffs { get; set; } = new();
    }

    public class PatientSensorDTO
    {
        public Guid PatientSensorID { get; set; }
        public string SensorKey { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public DateTime? UnassignedAt { get; set; }
        public bool IsActive { get; set; }

        public List<SensorDataDTO> SensorDatas { get; set; } = new();
    }

    public class PatientStaffDTO
    {
        public Guid PatientStaffID { get; set; }
        public string StaffIdentityNumber { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public DateTime? UnassignedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class SensorDataDTO
    {
        public BsonValue Value { get; set; } = BsonNull.Value;
        public DateTime RecordedAt { get; set; }
    }
}
