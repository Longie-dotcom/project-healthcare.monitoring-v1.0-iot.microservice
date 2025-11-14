namespace Application.DTO
{
    public class DeviceProfileControllerDTO
    {
        public string ControllerKey { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;
        public string FirmwareVersion { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;

        public string EdgeKey { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<DeviceProfileSensorDTO> Sensors { get; set; } = new();
    }

    public class DeviceProfileSensorDTO
    {
        public string SensorKey { get; set; }  = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
