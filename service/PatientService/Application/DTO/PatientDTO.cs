namespace Application.DTO
{
    public class PatientDTO
    {
        public Guid PatientID { get; set; }
        public string PatientCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public bool IsActive { get; set; }

        public string IdentityNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;

        public List<PatientBedAssignmentDTO>? PatientBedAssignments { get; set; }
        public List<PatientStaffAssignmentDTO>? PatientStaffAssignments { get; set; }
    }

    public class PatientBedAssignmentDTO
    {
        public Guid PatientBedAssignmentID { get; set; }
        public Guid PatientID { get; set; }
        public string ControllerKey { get; set; } = string.Empty;
        public DateTime? AssignedAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class PatientStaffAssignmentDTO
    {
        public Guid PatientStaffAssignmentID { get; set; }
        public Guid PatientID { get; set; }
        public string StaffIdentityNumber { get; set; } = string.Empty;
        public DateTime? AssignedAt { get; set; }
        public DateTime? UnassignedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class PatientCreateDTO
    {
        public string? PerformedBy { get; set; }
        public string IdentityNumber { get; set; } = string.Empty;
        public DateTime? AdmissionDate { get; set; }
    }

    public class PatientUpdateDTO
    {
        public string? PerformedBy { get; set; }
        public string PatientStatusCode { get; set; } = string.Empty;
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }

        public string? FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
    }

    public class PatientDeleteDTO
    {
        public string? PerformedBy { get; set; }
    }

    public class AssignStaffDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string StaffIdentityNumber { get; set; } = string.Empty;
    }

    public class UnassignStaffDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public Guid PatientStaffAssignmentID { get; set; }
    }

    public class AssignBedDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string ControllerKey { get; set; } = string.Empty;
    }

    public class ReleaseBedDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public Guid PatientBedAssignmentID { get; set; }
    }

    public class QueryPatientDTO
    {
        public string? Search { get; set; } = string.Empty;

        public int PageIndex { get; set; } = 1;
        public int PageLength { get; set; } = 10;
    }

    public class IAMSyncUpdateDTO
    {
        public string? PerformedBy { get; set; }
        public string IdentityNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
    }

    public class IAMSyncDeleteDTO
    {
        public string? PerformedBy { get; set; }
        public string IdentityNumber { get; set; } = string.Empty;
    }
}
