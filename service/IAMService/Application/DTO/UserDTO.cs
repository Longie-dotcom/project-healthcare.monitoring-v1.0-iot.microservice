namespace Application.DTO
{
    public class UserDTO
    {
        // For details
        public Guid UserID { get; set; }

        // For list view
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserDetailDTO
    {
        // For details
        public Guid UserID { get; set; }
        public List<UserRoleDTO> UserRoles { get; set; } = new List<UserRoleDTO>();
        public List<UserPrivilegeDTO> UserPrvileges { get; set; } = new List<UserPrivilegeDTO>();

        // For list view
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserRoleDTO
    {
        public RoleDetailDTO Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserPrivilegeDTO
    {
        public PrivilegeDTO Privilege { get; set; }
        public bool IsGranted { get; set; }
    }

    public class UserCreateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public List<string> RoleCodes { get; set; } = new List<string>();
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    public class UserUpdateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public List<UserRoleUpdateDTO> UserRoleUpdateDTOs { get; set; } = new();
        public List<UserPrivilegeUpdateDTO> UserPrivilegeUpdateDTOs { get; set; } = new();
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class UserRoleUpdateDTO
    {
        public Guid RoleID { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserPrivilegeUpdateDTO
    {
        public Guid PrivilegeID { get; set; }
        public bool IsGranted { get; set; }
    }

    public class UserDeleteDTO
    {
        public string PerformBy { get; set; } = string.Empty;
    }

    public class ChangePasswordDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string NewConfirmedPassword { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
    }

    public class QueryUserDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageLength { get; set; } = 10;
        public string? Search { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public DateTime? DateOfBirthFrom { get; set; }
        public DateTime? DateOfBirthTo { get; set; }
    }
}
