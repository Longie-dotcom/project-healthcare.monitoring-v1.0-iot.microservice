namespace HCM.MessageBrokerDTOs
{
    public class UpdateUser
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
    }
}
