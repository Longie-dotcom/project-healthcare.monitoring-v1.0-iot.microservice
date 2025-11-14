using HCM.MessageBrokerDTOs;

namespace Application.Interface.IMessagePublisher
{
    public interface IUpdateUserPublisher
    {
        Task PublishAsync(UpdateUser dto);
    }
}
