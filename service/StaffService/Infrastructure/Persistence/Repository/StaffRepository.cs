using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class StaffRepository :
        GenericRepository<Staff>,
        IStaffRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public StaffRepository(StaffDBContext context) : base(context) { }

        #region Methods
        public async Task<Staff?> GetDetailedByIdAsync(Guid id)
        {
            return await dbSet
                .AsTracking()
                .Include(s => s.StaffAssignments)
                .Include(s => s.StaffExperiences)
                .Include(s => s.StaffLicenses)
                .Include(s => s.StaffSchedules)
                .FirstOrDefaultAsync(s => s.StaffID == id && s.IsActive);
        }

        public async Task<IEnumerable<Staff>> GetAllWithFilter(
            string search, 
            int pageIndex, 
            int pageLength)
        {
            IQueryable<Staff> query = dbSet
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(s =>
                    s.StaffCode.ToLower().Contains(search) ||
                    s.IdentityNumber.ToLower().Contains(search) ||
                    s.ProfessionalTitle.ToLower().Contains(search) ||
                    s.Specialization.ToLower().Contains(search)
                );
            }

            // Apply paging
            query = query
                .OrderBy(s => s.StaffCode) // Or any default sort
                .Skip((pageIndex - 1) * pageLength)
                .Take(pageLength);

            return await query.ToListAsync();
        }

        public async Task<Staff?> GetStaffByIdentityNumber(string identityNumber)
        {
            return await dbSet
                .AsTracking()
                .FirstOrDefaultAsync(s => s.IdentityNumber == identityNumber);
        }

        public async Task UpdateAssignmentsAsync(
            Guid staffId, List<StaffAssignment> assignments)
        {
            var current = await context.StaffAssignments
                .Where(a => a.StaffID == staffId)
                .ToListAsync();

            context.StaffAssignments.RemoveRange(current);

            foreach (var a in assignments)
            {
                var entity = new StaffAssignment(
                    Guid.NewGuid(),
                    staffId,
                    a.Department,
                    a.Role,
                    a.StartDate,
                    a.EndDate,
                    a.IsActive
                );

                await context.StaffAssignments.AddAsync(entity);
            }
        }

        public async Task UpdateLicensesAsync(
            Guid staffId, List<StaffLicense> licenses)
        {
            var current = await context.StaffLicenses
                .Where(l => l.StaffID == staffId)
                .ToListAsync();

            context.StaffLicenses.RemoveRange(current);

            foreach (var l in licenses)
            {
                var entity = new StaffLicense(
                    Guid.NewGuid(),
                    staffId,
                    l.LicenseNumber,
                    l.LicenseType,
                    l.IssuedBy,
                    l.IssueDate,
                    l.ExpiryDate
                );

                await context.StaffLicenses.AddAsync(entity);
            }
        }

        public async Task UpdateExperiencesAsync(
            Guid staffId, List<StaffExperience> experiences)
        {
            var current = await context.StaffExperiences
                .Where(e => e.StaffID == staffId)
                .ToListAsync();

            context.StaffExperiences.RemoveRange(current);

            foreach (var e in experiences)
            {
                var entity = new StaffExperience(
                    Guid.NewGuid(),
                    staffId,
                    e.Institution,
                    e.Position,
                    e.StartYear,
                    e.EndYear,
                    e.Description
                );

                await context.StaffExperiences.AddAsync(entity);
            }
        }

        public async Task UpdateSchedulesAsync(
            Guid staffId, List<StaffSchedule> schedules)
        {
            var current = await context.StaffSchedules
                .Where(s => s.StaffID == staffId)
                .ToListAsync();

            context.StaffSchedules.RemoveRange(current);

            foreach (var s in schedules)
            {
                var entity = new StaffSchedule(
                    Guid.NewGuid(),
                    staffId,
                    s.DayOfWeek,
                    s.ShiftStart,
                    s.ShiftEnd,
                    s.IsActive
                );

                await context.StaffSchedules.AddAsync(entity);
            }
        }
        #endregion
    }
}
