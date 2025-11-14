using Application.DTO;

namespace Application.Interface.IMessagePublisher
{
    public interface IUpdateUserPublisher
    {
        Task PublishAsync(SyncUpdateUserDTO dto);
    }
}
