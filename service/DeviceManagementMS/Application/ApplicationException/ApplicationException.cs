namespace Application.ApplicationException
{
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message) : base(message) { }
    }

    public class DeviceNotFound : ApplicationExceptionBase
    {
        public DeviceNotFound(string message) : base(message) { }
    }

    public class RoomOccupied : ApplicationExceptionBase
    {
        public RoomOccupied(string message) : base(message) { }
    }

    public class BedOccupied : ApplicationExceptionBase
    {
        public BedOccupied(string message) : base(message) { }
    }
}
