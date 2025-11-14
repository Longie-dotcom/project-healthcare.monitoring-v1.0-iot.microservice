namespace Application.DTO
{
    public class EdgeDeviceSyncDTO
    {
        public string ControllerKey { get; set; } = string.Empty;
        public string? RoomName { get; set; }
        public string? IpAddress { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedBy { get; set; }
    }

    public class ControllerSyncDTO
    {
        public string ControllerKey { get; set; } = string.Empty;
        public string? BedNumber { get; set; }
        public string? IpAddress { get; set; }
        public string? FirmwareVersion { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedBy { get; set; }
    }

    public class SensorSyncDTO
    {
        public string SensorKey { get; set; } = string.Empty;
        public string ControllerKey { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? Unit { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedBy { get; set; }
    }
}