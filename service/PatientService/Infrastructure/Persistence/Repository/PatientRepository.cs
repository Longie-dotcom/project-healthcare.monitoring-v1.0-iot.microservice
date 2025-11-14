using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using Infrastructure.Persistence.Configuration;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class PatientRepository :
        GenericRepository<Patient>,
        IPatientRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public PatientRepository(PatientDBContext context) : base(context) { }

        #region Methods
        public async Task<Patient?> GetDetailedByIdAsync(Guid id)
        {
            return await dbSet
                .AsTracking()
                .Include(s => s.PatientStaffAssignment)
                .Include(s => s.PatientBedAssignments)
                .FirstOrDefaultAsync(p => p.PatientID == id && p.IsActive);
        }

        public async Task<IEnumerable<Patient>> GetAllWithFilter(
            string search, 
            int pageIndex, 
            int pageLength)
        {
            IQueryable<Patient> query = dbSet
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(s =>
                    s.PatientCode.ToLower().Contains(search) ||
                    s.IdentityNumber.ToLower().Contains(search)
                );
            }

            // Apply paging
            query = query
                .OrderBy(s => s.PatientCode) // Or any default sort
                .Skip((pageIndex - 1) * pageLength)
                .Take(pageLength);

            return await query.ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdentityNumber(string identityNumber)
        {
            return await dbSet
                .AsTracking()
                .FirstOrDefaultAsync(s => s.IdentityNumber == identityNumber);
        }

        public void AddStaffAssignment(PatientStaffAssignment assignment)
        {
            context.PatientStaffAssignments.Add(assignment);
        }

        public void RemoveStaffAssignment(PatientStaffAssignment assignment)
        {
            context.PatientStaffAssignments.Remove(assignment);
        }

        public void AddBedAssignment(PatientBedAssignment assignment)
        {
            context.PatientBedAssignments.Add(assignment);
        }

        public void RemoveBedAssignment(PatientBedAssignment assignment)
        {
            context.PatientBedAssignments.Remove(assignment);
        }

        public async Task<bool> IsControllerInUseAsync(string controllerKey)
        {
            if (string.IsNullOrWhiteSpace(controllerKey))
                return false;

            return await context.PatientBedAssignments
                .AsNoTracking()
                .AnyAsync(b => b.ControllerKey == controllerKey && b.ReleasedAt == null);
        }
        #endregion
    }
}
