namespace HCM.MessageBrokerDTOs
{
    // Publish from patient ms when a staff is assigned to a patient
    public class PatientStaffCreate
    {
        public string PatientIdentityNumber { get; set; } = string.Empty;
        public string StaffIdentityNumber { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
    }

    // Publish from patient ms when a staff is unassigned from a patient
    public class PatientStaffRemove
    {
        public string PatientIdentityNumber { get; set; } = string.Empty;
        public string StaffIdentityNumber { get; set; } = string.Empty;
        public DateTime UnassignedAt { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
    }
}
