using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface IPatientStatusRepository :
        IGenericRepository<PatientStatus>,
        IRepositoryBase
    {
        Task<PatientStatus> GetPatientStatusByCode(string code);

        Task DeletePatientStatusByCode(string code);
    }
}
