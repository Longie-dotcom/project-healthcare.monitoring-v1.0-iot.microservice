using Domain.Aggregate;
using Domain.IRepository;
using Infrastructure.Persistence.Configuration;

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

        public AuditLogRepository(PatientDBContext context) : base(context) { }
    }
}
