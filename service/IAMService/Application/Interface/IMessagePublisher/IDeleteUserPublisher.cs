using Application.DTO;

namespace Application.Interface.IMessagePublisher
{
    public interface IDeleteUserPublisher
    {
        Task PublishAsync(SyncDeleteUserDTO dto);
    }
}