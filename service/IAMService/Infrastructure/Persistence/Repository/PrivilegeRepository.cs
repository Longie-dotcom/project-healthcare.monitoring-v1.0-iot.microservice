using Domain.Aggregate;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class PrivilegeRepository : 
        GenericRepository<Privilege>, 
        IPrivilegeRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public PrivilegeRepository(IAMDBContext context) : base (context) { }

        #region Methods
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await context.Privileges.AnyAsync(p => p.Name == name);
        }

        public async Task<bool> ExistsByNameExceptIdAsync(string name, Guid privilegeId)
        {
            return await context.Privileges.AnyAsync(p => p.Name == name && p.PrivilegeID != privilegeId);
        }
        #endregion
    }
}
