using Domain.Aggregate;
using Domain.IRepository;

namespace Infrastructure.Persistence.Repository
{
    public class AuditLogRepository : 
        GenericRepository<AuditLog>, 
        IAuditLogRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public AuditLogRepository(IAMDBContext context): base(context) { }

        #region Methods
        #endregion
    }
}
