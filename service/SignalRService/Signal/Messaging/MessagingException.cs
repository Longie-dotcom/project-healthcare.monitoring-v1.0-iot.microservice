namespace Infrastructure.Messaging
{
    public abstract class MessagingException : Exception
    {
        protected MessagingException(string message) : base(message) { }
    }

    public class MessagingConnectionException : MessagingException
    {
        public MessagingConnectionException(string message) : base(message) { }
    }
}
