namespace SignalREnvelope
{
    public class SignalREnvelope
    {
        public string Topic { get; set; } = string.Empty;
        public string Method { get; set; } = "Receive";
        public string SourceService { get; set; } = string.Empty;
        public object Payload { get; set; } = default!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
