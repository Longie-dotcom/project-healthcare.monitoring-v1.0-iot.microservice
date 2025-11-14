namespace Application.DTO
{
    public class StaffDTO
    {
        public Guid StaffID { get; set; }
        public string StaffCode { get; set; } = string.Empty;
        public string ProfessionalTitle { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public string IdentityNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;

        public List<StaffLicenseDTO>? StaffLicenses { get; set; }
        public List<StaffScheduleDTO>? StaffSchedules { get; set; }
        public List<StaffAssignmentDTO>? StaffAssignments { get; set; }
        public List<StaffExperienceDTO>? StaffExperiences { get; set; }
    }

    public class StaffLicenseDTO
    {
        public Guid StaffLicenseID { get; set; }
        public Guid StaffID { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsValid { get; set; }
    }

    public class StaffScheduleDTO
    {
        public Guid StaffScheduleID { get; set; }
        public Guid StaffID { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan? ShiftStart { get; set; }
        public TimeSpan? ShiftEnd { get; set; }
        public bool IsActive { get; set; }
    }

    public class StaffAssignmentDTO
    {
        public Guid StaffAssignmentID { get; set; }
        public Guid StaffID { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class StaffExperienceDTO
    {
        public Guid StaffExperienceID { get; set; }
        public Guid StaffID { get; set; }
        public string Institution { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public string? Description { get; set; } = string.Empty;
    }

    public class StaffCreateDTO
    {
        public string? PerformedBy { get; set; }
        public string IdentityNumber { get; set; } = string.Empty;
        public string ProfessionalTitle { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;
    }

    public class StaffDeleteDTO
    {
        public string? PerformedBy { get; set; }
    }

    public class StaffUpdateDTO
    {
        public string? PerformedBy { get; set; }
        public string ProfessionalTitle { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;

        public string? FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;

        public List<StaffLicenseDTO>? StaffLicenses { get; set; }
        public List<StaffScheduleDTO>? StaffSchedules { get; set; }
        public List<StaffAssignmentDTO>? StaffAssignments { get; set; }
        public List<StaffExperienceDTO>? StaffExperiences { get; set; }
    }

    public class QueryStaffDTO
    {
        public string? Search { get; set; } = string.Empty;

        public int PageIndex { get; set; } = 1;
        public int PageLength { get; set; } = 10;
    }
}
