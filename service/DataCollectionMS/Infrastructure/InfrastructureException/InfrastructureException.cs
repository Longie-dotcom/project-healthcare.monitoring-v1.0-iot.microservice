namespace Infrastructure.InfrastructureException
{
    public abstract class InfrastructureExceptionBase : Exception
    {
        protected InfrastructureExceptionBase(string message) : base(message) { }
    }

    public class DatabaseConnectionException : InfrastructureExceptionBase
    {
        public DatabaseConnectionException(string message) : base(message) { }
    }

    public class MessagingConnectionException : InfrastructureExceptionBase
    {
        public MessagingConnectionException(string message) : base(message) { }
    }

    public class GrpcCommunicationException : InfrastructureExceptionBase
    {
        public GrpcCommunicationException(string message) : base(message) { }
    }

    public class InvalidJWTTokenException : InfrastructureExceptionBase
    {
        public InvalidJWTTokenException(string message) : base(message) { }
    }

    public class RepositoryException : InfrastructureExceptionBase
    {
        public RepositoryException(string message) : base(message) { }
    }
}
