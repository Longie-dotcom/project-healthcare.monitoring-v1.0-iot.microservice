namespace HCM.Authorization
{
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException(string privilege)
            : base($"You do not have required privilege: {privilege}") { }
    }
}
