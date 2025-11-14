namespace HCM.MessageBrokerDTOs
{
    // Publish from patient ms when assign new controller to a patient
    public class DeviceProfileCreate
    {
        // Patient snapshot
        public string IdentityNumber { get; set; } = string.Empty; // map with patient ms
        public string FullName { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? AssignedAt { get; set; }
        public List<PatientStaffDTO> PatientStaffs { get; set; } = new();

        public string EdgeKey { get; set; } = string.Empty;
        public string ControllerKey { get; set; } = string.Empty; // map with device management ms
        public string IpAddress { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<PatientSensorDTO> PatientSensors { get; set; } = new();
        public string? PerformedBy { get; set; }
    }

    public class PatientSensorDTO
    {
        public string SensorKey { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }

    public class PatientStaffDTO
    {
        public string StaffIdentityNumber { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }

    // Publish from patient ms when unassign a controller from a patient
    public class DeviceProfileRemove
    {
        public string IdentityNumber { get; set; } = string.Empty;
        public string EdgeKey { get; set; } = string.Empty;
        public string ControllerKey { get; set; } = string.Empty;
        public DateTime UnassignedAt { get; set; } = DateTime.UtcNow;
        public string? PerformedBy { get; set; }
    }
}
