namespace Domain.DomainException
{
    public abstract class DomainExceptionBase : Exception
    {
        protected DomainExceptionBase(string message) : base(message) { }
    }

    public class InvalidEdgeDeviceAggregateException : DomainExceptionBase
    {
        public InvalidEdgeDeviceAggregateException(string message) 
            : base(message) { }
    }
}
