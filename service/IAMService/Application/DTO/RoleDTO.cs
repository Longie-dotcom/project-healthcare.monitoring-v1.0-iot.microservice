namespace Application.DTO
{
    public class RoleDTO
    {
        public Guid RoleID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public List<Guid> PrivilegeID { get; set; } = new List<Guid>();
    }

    public class RoleDetailDTO
    {
        public Guid RoleID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public List<PrivilegeDTO> Privileges { get; set; }
            = new List<PrivilegeDTO>();
    }

    public class RoleCreateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public List<Guid> PrivilegeID { get; set; } = new List<Guid>();
    }

    public class RoleUpdateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Guid> PrivilegeID { get; set; } = new List<Guid>();
    }

    public class RoleDeleteDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
    }
}
