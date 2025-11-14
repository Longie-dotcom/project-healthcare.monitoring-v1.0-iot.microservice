using Application.DTO;

namespace Application.Interface.IService
{
    public interface IPrivilegeService
    {
        Task<PrivilegeDTO> GetPrivilegeByIdAsync(Guid privilegeId);
        Task<IEnumerable<PrivilegeDTO>> GetPrivilegesAsync();

        Task<PrivilegeDTO> CreatePrivilegeAsync(PrivilegeCreateDTO dto);
        Task<PrivilegeDTO> UpdatePrivilegeAsync(Guid PrivilegeId, PrivilegeUpdateDTO dto);
        Task DeletePrivilegeAsync(Guid PrivilegeId, UserDeleteDTO dto);
    }
}
