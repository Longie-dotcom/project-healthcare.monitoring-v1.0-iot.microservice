using Application.DTO;

namespace Application.IGrpc
{
    public interface IIAMGrpcClient
    {
        Task<UserDTO> GetUser(string identityNumber);
    }
}
