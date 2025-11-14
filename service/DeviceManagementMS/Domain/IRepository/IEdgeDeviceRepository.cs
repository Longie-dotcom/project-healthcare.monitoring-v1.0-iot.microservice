using Domain.Aggregate;
using Domain.Entity;

namespace Domain.IRepository
{
    public interface IEdgeDeviceRepository :
        IGenericRepository<EdgeDevice>,
        IRepositoryBase
    {
        // Queries
        Task<EdgeDevice> GetDetailedByIdAsync(Guid id);
        Task<IEnumerable<EdgeDevice>> GetAllWithFilter(string search, int pageIndex, int pageLength);
        Task<EdgeDevice> GetEdgeDeviceByEdgeKey(string edgeKey);
        Task<EdgeDevice> GetEdgeDeviceByEdgeKeyAsTracking(string edgeKey);

        // Persistence of assignment (aggregate ensures rules)
        void AssignController(Controller controller);
        void AssignSensor(Sensor sensor);

        // Helper for room validation
        Task<bool> IsRoomAssignedAsync(string roomName);
    }
}
