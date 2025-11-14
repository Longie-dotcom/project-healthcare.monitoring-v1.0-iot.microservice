using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface IAuditLogRepository : 
        IGenericRepository<AuditLog>,
        IRepositoryBase
    {
    }
}
