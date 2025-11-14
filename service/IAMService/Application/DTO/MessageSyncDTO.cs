namespace Application.DTO
{
    public class SyncUpdateUserDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class SyncDeleteUserDTO
    {
        public string IdentityNumber { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
    }
}
