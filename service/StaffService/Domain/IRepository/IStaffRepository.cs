using Domain.Aggregate;
using Domain.Entity;

namespace Domain.IRepository
{
    public interface IStaffRepository : 
        IGenericRepository<Staff>, 
        IRepositoryBase
    {
        Task<Staff> GetDetailedByIdAsync(Guid id);
        Task<IEnumerable<Staff>> GetAllWithFilter(string search, int pageIndex, int pageLength);

        Task<Staff> GetStaffByIdentityNumber(string identityNumber);

        Task UpdateLicensesAsync(Guid staffId, List<StaffLicense> newLicenses);
        Task UpdateSchedulesAsync(Guid staffId, List<StaffSchedule> newSchedules);
        Task UpdateAssignmentsAsync(Guid staffId, List<StaffAssignment> newAssignments);
        Task UpdateExperiencesAsync(Guid staffId, List<StaffExperience> newExperiences);
    }
}
