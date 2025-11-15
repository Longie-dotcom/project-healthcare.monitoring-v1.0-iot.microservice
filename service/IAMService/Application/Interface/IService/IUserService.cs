using Application.DTO;
using HCM.MessageBrokerDTOs;

namespace Application.Interface.IService
{
    public interface IUserService
    {
        Task<UserDTO> GetByIdentityNumberAsync(string identityNumber);
        Task<UserDTO> GetByPhoneAsync(string phone);
        Task<UserDTO> GetByEmailAsync(string email);
        Task<IEnumerable<UserDTO>> GetUsersAsync(string? sortBy, QueryUserDTO dto);
        Task<UserDetailDTO?> GetUserByIdAsync(Guid userId);

        Task<UserDTO> CreateUserAsync(UserCreateDTO dto);
        Task<UserDTO> UpdateUserAsync(Guid userId, UserUpdateDTO dto);
        Task DeleteUserAsync(Guid userId, UserDeleteDTO dto);
        Task ChangePasswordAsync(Guid userId, ChangePasswordDTO dto);

        Task SyncUserUpdate(UpdateUser dto);
    }
}