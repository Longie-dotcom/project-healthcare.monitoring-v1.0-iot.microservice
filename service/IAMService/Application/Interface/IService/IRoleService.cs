using Application.DTO;

namespace Application.Interface.IService
{
    public interface IRoleService
    {
        Task<RoleDetailDTO> GetRoleByIdAsync(Guid roleId);
        Task<IEnumerable<RoleDTO>> GetRoleListAsync();

        Task<RoleDTO> CreateRoleAsync(RoleCreateDTO dto);
        Task<RoleDTO> UpdateRoleAsync(Guid roleId, RoleUpdateDTO dto);
        Task DeleteRoleAsync(Guid roleId, UserDeleteDTO dto);
    }
}
