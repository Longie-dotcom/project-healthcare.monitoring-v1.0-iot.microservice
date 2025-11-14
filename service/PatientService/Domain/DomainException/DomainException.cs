namespace Domain.DomainException
{
    public abstract class DomainExceptionBase : Exception
    {
        protected DomainExceptionBase(string message) : base(message) { }
    }

    public class InvalidPatientAggregateException : DomainExceptionBase
    {
        public InvalidPatientAggregateException(string message) 
            : base(message) { }
    }
}
