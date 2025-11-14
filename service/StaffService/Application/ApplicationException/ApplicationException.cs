namespace Application.ApplicationException
{
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message) : base(message) { }
    }

    public class StaffNotFound : ApplicationExceptionBase
    {
        public StaffNotFound(string message) : base(message) { }
    }

    public class UserNotFound : ApplicationExceptionBase
    {
        public UserNotFound(string message) : base(message) { }
    }

    public class IdentityNumberExisted : ApplicationExceptionBase
    {
        public IdentityNumberExisted(string message) : base(message) { }
    }

    public class StaffCodeExisted : ApplicationExceptionBase
    {
        public StaffCodeExisted(string message) : base(message) { }
    }

    public class LicenseNumberExisted : ApplicationExceptionBase
    {
        public LicenseNumberExisted(string message) : base(message) { }
    }
}
