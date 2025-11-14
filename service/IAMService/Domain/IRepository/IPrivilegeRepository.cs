using Domain.Aggregate;
namespace Domain.IRepository
{
    public interface IPrivilegeRepository : 
        IGenericRepository<Privilege>,
        IRepositoryBase
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByNameExceptIdAsync(string name, Guid privilegeId);
    }
}
