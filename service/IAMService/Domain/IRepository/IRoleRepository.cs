using Domain.Aggregate;
namespace Domain.IRepository
{
    public interface IRoleRepository : 
        IGenericRepository<Role>, 
        IRepositoryBase
    {
        Task<Role> GetByCodeAsync(string code);
        Task<IEnumerable<Role>> GetRolesWithFilterAsync();
        Task<Role> GetByDetailByIdAsync(Guid roleId);

        Task UpdateRolePrivilegesAsync(Guid roleId, IEnumerable<Guid> newPrivilegeIds);
    }
}
