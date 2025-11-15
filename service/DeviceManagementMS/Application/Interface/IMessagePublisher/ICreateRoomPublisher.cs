using HCM.MessageBrokerDTOs;

namespace Application.Interface.IMessagePublisher
{
    public interface ICreateRoomPublisher
    {
        Task PublishCreateRoomAsync(DeviceCreate dto);
    }
}
