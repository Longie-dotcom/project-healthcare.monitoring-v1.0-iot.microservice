using Application.DTO;

namespace Application.Interface.IMessagePublisher
{
    public interface ICreateDeviceProfilePublisher
    {
        Task PublishDeviceProfileAsync(CreateDeviceProfileDTO dto);
    }
}
