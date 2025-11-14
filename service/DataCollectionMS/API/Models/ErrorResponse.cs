namespace API.Models
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}