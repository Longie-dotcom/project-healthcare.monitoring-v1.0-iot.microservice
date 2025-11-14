using Domain.Aggregate;
using Domain.IRepository;
using Infrastructure.Persistence.Configuration;

namespace Infrastructure.Persistence.Repository
{
    public class PatientStatusRepository : 
        GenericRepository<PatientStatus>,
        IPatientStatusRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public PatientStatusRepository(PatientDBContext context) : base(context) { }

        #region Methods
        public async Task<PatientStatus?> GetPatientStatusByCode(string code)
        {
            return await dbSet.FindAsync(code);
        }

        public async Task DeletePatientStatusByCode(string code)
        {
            var existing = await dbSet.FindAsync(code);
            if (existing != null)
                dbSet.Remove(existing);
        }
        #endregion
    }
}
