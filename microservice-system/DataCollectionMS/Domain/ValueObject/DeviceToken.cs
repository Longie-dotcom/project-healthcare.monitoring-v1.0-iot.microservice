namespace Domain.ValueObject
{
    public class DeviceToken
    {
        public string Token { get; private set; }

        protected DeviceToken() { }

        public DeviceToken(string token)
        {
            Token = token;
        }
    }
}
