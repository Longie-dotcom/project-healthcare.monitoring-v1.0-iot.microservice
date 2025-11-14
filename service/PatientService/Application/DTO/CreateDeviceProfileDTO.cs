namespace Application.DTO
{
    public class CreateDeviceProfileDTO
    {
        public string PatientIdentityNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? BedAssignedAt { get; set; }
        public DateTime? BedReleasedAt { get; set; }

        public string ControllerKey { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;
        public string EdgeKey { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string FirmwareVersion { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<PatientSensorDTO> PatientSensors { get; set; } = new List<PatientSensorDTO>();
        public List<PatientStaffDTO> PatientStaffs { get; set; } = new List<PatientStaffDTO>();
    }

    public class PatientSensorDTO
    {
        public string SensorKey { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public DateTime? UnassignedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class PatientStaffDTO
    {
        public string StaffIdentityNumber { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public DateTime? UnassignedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
