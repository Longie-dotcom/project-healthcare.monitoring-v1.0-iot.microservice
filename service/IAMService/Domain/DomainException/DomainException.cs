namespace Domain.DomainException
{
    public abstract class DomainExceptionBase : Exception
    {
        protected DomainExceptionBase(string message) : base(message) { }
    }

    public class InvalidUserAggregateException : DomainExceptionBase
    {
        public InvalidUserAggregateException(string message) : base(message) { }
    }

    public class InvalidRoleAggregateException : DomainExceptionBase
    {
        public InvalidRoleAggregateException(string message) : base(message) { }
    }

    public class InvalidPrivilegeAggregateException : DomainExceptionBase
    {
        public InvalidPrivilegeAggregateException(string message) : base(message) { }
    }

    public class InvalidPasswordOVException : DomainExceptionBase
    {
        public InvalidPasswordOVException(string message) : base(message) { }
    }
}
