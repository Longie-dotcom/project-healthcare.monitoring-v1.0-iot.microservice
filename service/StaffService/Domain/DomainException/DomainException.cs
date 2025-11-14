namespace Domain.DomainException
{
    public abstract class DomainExceptionBase : Exception
    {
        protected DomainExceptionBase(string message) : base(message) { }
    }

    public class InvalidStaffAggregateException : DomainExceptionBase
    {
        public InvalidStaffAggregateException(string message) 
            : base(message) { }
    }
}
