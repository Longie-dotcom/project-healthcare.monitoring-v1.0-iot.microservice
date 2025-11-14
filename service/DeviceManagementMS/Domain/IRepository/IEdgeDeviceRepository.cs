using Domain.Aggregate;
using Domain.Entity;

namespace Domain.IRepository
{
    public interface IEdgeDeviceRepository : 
        IGenericRepository<EdgeDevice>, 
        IRepositoryBase
    {
        Task<EdgeDevice> GetDetailedByIdAsync(Guid id);
        Task<IEnumerable<EdgeDevice>> GetAllWithFilter(
            string search, int pageIndex, int pageLength);

        Task<EdgeDevice> GetEdgeDeviceByEdgeKey(string edgeKey);
        Task<EdgeDevice> GetEdgeDeviceByEdgeKeyAsTracking(string edgeKey);
        Task<Controller?> GetControllerByControllerKey(string controllerKey);
        Task<Controller?> GetControllerByControllerKeyAsTracking(string controllerKey);
        Task<Sensor?> GetSensorBySensorKeyAsTracking(string sensorKey);

        Task<EdgeDevice> GetEdgeDeviceByEdgeKeyAsTrackingInactive(string edgeKey);
        Task<Controller?> GetControllerByControllerKeyAsTrackingInactive(string controllerKey);
        Task<Sensor?> GetSensorBySensorKeyAsTrackingInactive(string sensorKey);

        Task<bool> IsBedAssignedAsync(string edgeKey, string bedNumber);
        Task<bool> IsRoomAssignedAsync(string roomName);

        void AssignSensor(Sensor sensor);
        void AssignController(Controller controller);
    }
}
