namespace HCM.MessageBrokerDTOs
{
    // Publish from device management ms when a sensor is created
    public class PatientSensorCreate
    {
        public string EdgeKey { get; set; } = string.Empty;
        public string ControllerKey { get; set; } = string.Empty;
        public string SensorKey { get; set; } = string.Empty;
        public string? PerformedBy { get; set; }
    }

    // Publish from device management ms when a sensor is removed
    public class PatientSensorRemove
    {
        public string EdgeKey { get; set; } = string.Empty;
        public string ControllerKey { get; set; } = string.Empty;
        public string SensorKey { get; set; } = string.Empty;
        public DateTime UnassignedAt { get; set; } = DateTime.UtcNow;
        public string? PerformedBy { get; set; }
    }
}
