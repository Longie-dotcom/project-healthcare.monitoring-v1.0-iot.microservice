namespace API.Models
{
    public class SuccessResponse
    {
        public bool Success { get; set; } = true;
        public object? Payload { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}