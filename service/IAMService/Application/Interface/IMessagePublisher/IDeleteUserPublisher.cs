using Application.DTO;
using HCM.MessageBrokerDTOs;

namespace Application.Interface.IMessagePublisher
{
    public interface IDeleteUserPublisher
    {
        Task PublishAsync(DeleteUser dto);
    }
}