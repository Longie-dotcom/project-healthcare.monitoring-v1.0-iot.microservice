using HCM.MessageBrokerDTOs;

namespace Application.Interface.IMessagePublisher
{
    public interface IUpdateDevicePublisher
    {
        Task PublishUpdateEdgeAsync(UpdateEdgeDeviceDTO dto);
        Task PublishUpdateControllerAsync(UpdateControllerDTO dto);
        Task PublishUpdateSensorAsync(UpdateSensorDTO dto);
    }
}
