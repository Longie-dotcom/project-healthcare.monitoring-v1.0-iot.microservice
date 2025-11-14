using Domain.Aggregate;
using Domain.Entity;

namespace Domain.IRepository
{
    public interface IPatientRepository : 
        IGenericRepository<Patient>, 
        IRepositoryBase
    {
        Task<Patient> GetDetailedByIdAsync(Guid id);
        Task<IEnumerable<Patient>> GetAllWithFilter(string search, int pageIndex, int pageLength);
        Task<Patient?> GetPatientByIdentityNumber(string identityNumber);

        void AddStaffAssignment(PatientStaffAssignment assignment);
        void RemoveStaffAssignment(PatientStaffAssignment assignment);
        void AddBedAssignment(PatientBedAssignment assignment);
        void RemoveBedAssignment(PatientBedAssignment assignment);

        Task<bool> IsControllerInUseAsync(string controllerKey);
    }
}
