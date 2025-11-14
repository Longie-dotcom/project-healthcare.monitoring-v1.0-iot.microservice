namespace HCM.MessageBrokerDTOs
{
    public class UpdateEdgeDeviceDTO
    {
        public string EdgeKey { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? RoomName { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedBy { get; set; }
    }

    public class UpdateControllerDTO
    {
        public string EdgeKey { get; set; } = string.Empty;
        public string ControllerKey { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? BedNumber { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedBy { get; set; }
    }

    public class UpdateSensorDTO
    {
        public string EdgeKey { get; set; } = string.Empty;
        public string ControllerKey { get; set; } = string.Empty;
        public string SensorKey { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public string? PerformedBy { get; set; }
    }
}
