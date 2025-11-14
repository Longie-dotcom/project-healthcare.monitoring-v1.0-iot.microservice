using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class RoleRepository : 
        GenericRepository<Role>, 
        IRoleRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion
        public RoleRepository(IAMDBContext context) : base(context) { }

        #region Methods
        public async Task<Role?> GetByCodeAsync(string code)
        {
            var role = await context.Roles
                .AsNoTracking()
                .Include(r => r.RolePrivileges)
                    .ThenInclude(rq => rq.Privilege)
                .FirstOrDefaultAsync(r => r.Code == code);
                
            return role;
        }

        public async Task<IEnumerable<Role>> GetRolesWithFilterAsync()
        {
            var roles = await context.Roles
                .Include(r => r.RolePrivileges)
                    .ThenInclude(rp => rp.Privilege)
                .AsNoTracking()
                .ToListAsync();

            return roles;
        }

        public async Task<Role?> GetByDetailByIdAsync(Guid roleId)
        {
            var role = await context.Roles
                .AsNoTracking()
                .Include(r => r.RolePrivileges)
                    .ThenInclude(rq => rq.Privilege)
                .FirstOrDefaultAsync(r => r.RoleID == roleId);

            return role;
        }

        public async Task UpdateRolePrivilegesAsync(Guid roleId, IEnumerable<Guid> newPrivilegeIds)
        {
            var role = await context.Roles
                .Include(r => r.RolePrivileges)
                .FirstOrDefaultAsync(r => r.RoleID == roleId);

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleId} not found");

            // Remove all existing privileges
            context.RemoveRange(role.RolePrivileges);

            // Add new privileges
            foreach (var pid in newPrivilegeIds.Distinct())
            {
                context.Add(new RolePrivilege(Guid.NewGuid(), roleId, pid, true));
            }

            await context.SaveChangesAsync();
        }
        #endregion
    }
}
