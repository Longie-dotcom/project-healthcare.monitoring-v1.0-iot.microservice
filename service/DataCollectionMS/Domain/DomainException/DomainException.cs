namespace Domain.DomainException
{
    public abstract class DomainExceptionBase : Exception
    {
        protected DomainExceptionBase(string message) : base(message) { }
    }

    public class InvalidRoomProfileAggregateException : DomainExceptionBase
    {
        public InvalidRoomProfileAggregateException(string message) : base(message) { }
    }
}
