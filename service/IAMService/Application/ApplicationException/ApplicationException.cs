namespace Application.ApplicationException
{
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message) : base(message) { }
    }

    public class RoleCodeNotFound : ApplicationExceptionBase
    {
        public RoleCodeNotFound(string roleCode)
            : base($"Role with code '{roleCode}' was not found.") { }
    }

    public class RoleGuidNotFound : ApplicationExceptionBase
    {
        public RoleGuidNotFound(Guid roleCode)
            : base($"Role with code '{roleCode}' was not found.") { }
    }

    public class UserNotFound : ApplicationExceptionBase
    {
        public UserNotFound()
            : base($"User was not found.") { }
    }

    public class UserEmailNotFound : ApplicationExceptionBase
    {
        public UserEmailNotFound(string email)
            : base($"User with email '{email}' was not found.") { }
    }

    public class PrivilegeNotFound : ApplicationExceptionBase
    {
        public PrivilegeNotFound(string privilegeCode)
            : base($"Privilege with code '{privilegeCode}' was not found.") { }
    }

    public class UserAlreadyExists : ApplicationExceptionBase
    {
        public UserAlreadyExists(string message)
            : base(message) { }
    }

    public class RoleCodeAlreadyExists : ApplicationExceptionBase
    {
        public RoleCodeAlreadyExists(string roleCode)
            : base($"Role with code '{roleCode}' already exists.") { }
    }

    public class InvalidTokenException : ApplicationExceptionBase
    {
        public InvalidTokenException(string message)
            : base(message) { }
    }

    public class InvalidPassword : ApplicationExceptionBase
    {
        public InvalidPassword()
            : base("The password provided does not match this account.") { }
    }

    public class InvalidRole : ApplicationExceptionBase
    {
        public InvalidRole()
            : base("User has no permission to this role") { }
    }

    public class InvalidResetPassword : ApplicationExceptionBase
    {
        public InvalidResetPassword(string message)
            : base(message) { }
    }

    public class InvalidChangePassword : ApplicationExceptionBase
    {
        public InvalidChangePassword(string message)
            : base(message) { }
    }
    public class PrivilegeAlreadyExists : Exception
    {
        public PrivilegeAlreadyExists(string message) : base(message) { }
    }
}
