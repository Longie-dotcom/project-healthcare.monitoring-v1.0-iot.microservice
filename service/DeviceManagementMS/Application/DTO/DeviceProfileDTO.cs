namespace Application.DTO
{
    public class DeviceProfileControllerDTO
    {
        public string ControllerKey { get; set; }
        public string BedNumber { get; set; }
        public string FirmwareVersion { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }

        public string EdgeKey { get; set; }
        public string RoomName { get; set; }
        public string IpAddress { get; set; }
        public string Description { get; set; }

        public List<DeviceProfileSensorDTO> Sensors { get; set; } = new();
    }

    public class DeviceProfileSensorDTO
    {
        public string SensorKey { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
