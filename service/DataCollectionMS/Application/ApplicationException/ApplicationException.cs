namespace Application.ApplicationException
{
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message) : base(message) { }
    }

    public class PatientExisted : ApplicationExceptionBase
    {
        public PatientExisted(string identityNumber)
            : base($"Patient with identity number: '{identityNumber}' is existed.") { }
    }

    public class RoomProfileNotFound : ApplicationExceptionBase
    {
        public RoomProfileNotFound(string message)
            : base(message) { }
    }

    public class DeviceProfileNotFound : ApplicationExceptionBase
    {
        public DeviceProfileNotFound(string message)
            : base(message) { }
    }

    public class PatientSensorNotFound : ApplicationExceptionBase
    {
        public PatientSensorNotFound(string message)
            : base(message) { }
    }

    public class StaffAssignmentNotFound : ApplicationExceptionBase
    {
        public StaffAssignmentNotFound(string message)
            : base(message) { }
    }

    public class PatientAssignmentNotFound : ApplicationExceptionBase
    {
        public PatientAssignmentNotFound(string message)
            : base(message) { }
    }

    public class UnauthorizedAssignment : ApplicationExceptionBase
    {
        public UnauthorizedAssignment(string message)
            : base(message) { }
    }
}
