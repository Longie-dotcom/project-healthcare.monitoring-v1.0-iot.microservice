using Application.DTO;

namespace Application.Interface.IService
{
    public interface IPatientStatusService
    {
        Task<PatientStatusDTO> GetByCodeAsync(string statusCode);
        Task<IEnumerable<PatientStatusDTO>> GetAllAsync();
        Task<PatientStatusDTO> CreateAsync(PatientStatusCreateDTO dto);
        Task<PatientStatusDTO> UpdateAsync(string statusCode, PatientStatusUpdateDTO dto);
        Task DeleteAsync(string statusCode, PatientStatusDeleteDTO dto);
    }
}
