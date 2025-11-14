namespace Application.DTO
{
    public class PrivilegeDTO
    {
        public Guid PrivilegeID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PrivilegeCreateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PrivilegeUpdateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PrivilegeDeleteDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
    }
}
