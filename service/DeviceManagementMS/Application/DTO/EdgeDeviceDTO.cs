namespace Application.DTO
{
    public class EdgeDeviceDTO
    {
        public Guid EdgeDeviceID { get; set; }
        public string EdgeKey { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<ControllerDTO>? Controllers { get; set; }
    }

    public class ControllerDTO
    {
        public Guid ControllerID { get; set; }
        public string ControllerKey { get; set; } = string.Empty;
        public string EdgeKey { get; set; } = string.Empty;
        public string? BedNumber { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string? FirmwareVersion { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<SensorDTO>? Sensors { get; set; }
    }

    public class SensorDTO
    {
        public Guid SensorID { get; set; }
        public string SensorKey { get; set; } = string.Empty;
        public string ControllerKey { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class EdgeDeviceCreateDTO
    {
        public string? PerformedBy { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }

    public class EdgeDeviceUpdateDTO
    {
        public string? PerformedBy { get; set; }
        public string? RoomName { get; set; }
        public string? IpAddress { get; set; }
        public string? Description { get; set; }
    }

    public class EdgeDeviceDeleteDTO
    {
        public string? PerformedBy { get; set; }
    }

    public class QueryEdgeDeviceDTO
    {
        public string? Search { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageLength { get; set; } = 10;
    }

    public class ControllerCreateDTO
    {
        public string EdgeKey { get; set; } = string.Empty;
        public string? BedNumber { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string? FirmwareVersion { get; set; } = string.Empty;
        public string? PerformedBy { get; set; }
    }

    public class SensorCreateDTO
    {
        public string ControllerKey { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? PerformedBy { get; set; }
    }

    public class ControllerDeleteDTO
    {
        public string ControllerKey { get; set; } = string.Empty;
        public string? PerformedBy { get; set; }
    }

    public class SensorDeleteDTO
    {
        public string SensorKey { get; set; } = string.Empty;
        public string? PerformedBy { get; set; }
    }
}
