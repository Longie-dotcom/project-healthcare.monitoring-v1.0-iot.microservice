using Application.DTO;

namespace Application.Interface.IMessagePublisher
{
    public interface IUpdateDevicePublisher
    {
        Task PublishUpdateEdgeAsync(EdgeDeviceSyncDTO dto);
        Task PublishUpdateControllerAsync(ControllerSyncDTO dto);
        Task PublishUpdateSensorAsync(SensorSyncDTO dto);
    }
}
