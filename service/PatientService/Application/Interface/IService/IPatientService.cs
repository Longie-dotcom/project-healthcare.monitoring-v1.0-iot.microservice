using Application.DTO;

namespace Application.Interface.IService
{
    public interface IPatientService
    {
        Task<PatientDTO> GetByIdAsync(Guid patientId);
        Task<IEnumerable<PatientDTO>> GetAllAsync(QueryPatientDTO dto, string sort);
        Task<PatientDTO> CreateAsync(PatientCreateDTO dto);
        Task<PatientDTO> UpdateAsync(Guid patientId, PatientUpdateDTO dto);
        Task SyncUpdateAsync(IAMSyncUpdateDTO dto);
        Task DeleteAsync(Guid patientId, PatientDeleteDTO dto);
        Task SyncDeleteAsync(IAMSyncDeleteDTO dto);

        Task AssignBedAsync(Guid patientId, AssignBedDTO dto);
        Task ReleaseBedAsync(Guid patientId, ReleaseBedDTO dto);
        Task AssignStaffAsync(Guid patientId, AssignStaffDTO dto);
        Task UnassignStaffAsync(Guid patientId, UnassignStaffDTO dto);
    }
}
