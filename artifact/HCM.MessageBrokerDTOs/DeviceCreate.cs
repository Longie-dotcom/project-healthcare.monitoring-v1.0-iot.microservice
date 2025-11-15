namespace HCM.MessageBrokerDTOs
{
    public class DeviceCreate
    {
        public string EdgeKey { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? RoomName { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedBy { get; set; }
    }
}
