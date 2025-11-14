namespace Application.ApplicationException
{
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message) : base(message) { }
    }

    public class PatientNotFound : ApplicationExceptionBase
    {
        public PatientNotFound(string message) : base(message) { }
    }

    public class UserNotFound : ApplicationExceptionBase
    {
        public UserNotFound(string message) : base(message) { }
    }

    public class DeviceNotFound : ApplicationExceptionBase
    {
        public DeviceNotFound(string message) : base(message) { }
    }

    public class PatientStatusCodeNotFound : ApplicationExceptionBase
    {
        public PatientStatusCodeNotFound(string message) : base(message) { }
    }

    public class IdentityNumberExisted : ApplicationExceptionBase
    {
        public IdentityNumberExisted(string message) : base(message) { }
    }

    public class PatientCodeExisted : ApplicationExceptionBase
    {
        public PatientCodeExisted(string message) : base(message) { }
    }

    public class PatientStatusCodeExisted : ApplicationExceptionBase
    {
        public PatientStatusCodeExisted(string message) : base(message) { }
    }
}
