using Application.DTO;

namespace Application.Interface.IService
{
    public interface IStaffService
    {
        Task<StaffDTO> GetByIdAsync(Guid staffId);
        Task<IEnumerable<StaffDTO>> GetAllAsync(QueryStaffDTO dto, string sort);
        Task<StaffDTO> CreateAsync(StaffCreateDTO dto);
        Task<StaffDTO> UpdateAsync(Guid staffId, StaffUpdateDTO dto);
        Task SyncUpdateAsync(IAMSyncUpdateDTO dto);
        Task DeleteAsync(Guid staffId, StaffDeleteDTO dto);
        Task SyncDeleteAsync(IAMSyncDeleteDTO dto);
    }
}
