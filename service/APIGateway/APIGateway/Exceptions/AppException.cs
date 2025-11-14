namespace APIGateway.Exceptions
{
    public class AppException(int statusCode, string message, string? errorCode = null) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
        public string? ErrorCode { get; } = errorCode;
    }
}
