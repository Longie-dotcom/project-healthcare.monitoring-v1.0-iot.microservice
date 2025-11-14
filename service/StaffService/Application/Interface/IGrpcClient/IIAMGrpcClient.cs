using Application.DTO;

namespace Application.Interface.IGrpc
{
    public interface IIAMGrpcClient
    {
        Task<UserDTO> GetUser(string identityNumber);
    }
}
