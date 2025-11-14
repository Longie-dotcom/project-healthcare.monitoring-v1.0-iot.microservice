using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class EdgeDeviceRepository :
        GenericRepository<EdgeDevice>,
        IEdgeDeviceRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public EdgeDeviceRepository(DeviceManagementDBContext context) : base(context) { }

        #region Methods
        public async Task<EdgeDevice?> GetDetailedByIdAsync(Guid id)
        {
            return await context.EdgeDevices
                .AsTracking()
                .Include(e => e.Controllers)
                    .ThenInclude(c => c.Sensors)
                .FirstOrDefaultAsync(e => e.EdgeDeviceID == id && e.IsActive);
        }

        public async Task<IEnumerable<EdgeDevice>> GetAllWithFilter(
            string search, int pageIndex, int pageLength)
        {
            IQueryable<EdgeDevice> query = context.EdgeDevices.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(e =>
                    e.EdgeKey.ToLower().Contains(search) ||
                    e.RoomName.ToLower().Contains(search) ||
                    e.IpAddress.ToLower().Contains(search) ||
                    e.Description.ToLower().Contains(search)
                );
            }

            query = query
                .OrderBy(e => e.EdgeKey)
                .Skip((pageIndex - 1) * pageLength)
                .Take(pageLength);

            return await query.ToListAsync();
        }

        public async Task<EdgeDevice?> GetEdgeDeviceByEdgeKey(string edgeKey)
        {
            return await context.EdgeDevices
                .AsNoTracking()
                .Include(e => e.Controllers)
                    .ThenInclude(c => c.Sensors)
                .FirstOrDefaultAsync(e => e.EdgeKey == edgeKey && e.IsActive);
        }

        public async Task<EdgeDevice?> GetEdgeDeviceByEdgeKeyAsTracking(string edgeKey)
        {
            return await context.EdgeDevices
                .AsTracking()
                .Include(e => e.Controllers)
                    .ThenInclude(c => c.Sensors)
                .FirstOrDefaultAsync(e => e.EdgeKey == edgeKey && e.IsActive);
        }

        public async Task<Controller?> GetControllerByControllerKey(string controllerKey)
        {
            return await context.Controllers
                .AsNoTracking()
                .Include(c => c.Sensors)
                .FirstOrDefaultAsync(e => e.ControllerKey == controllerKey && e.IsActive);
        }

        public async Task<Controller?> GetControllerByControllerKeyAsTracking(string controllerKey)
        {
            return await context.Controllers
                .AsTracking()
                .Include(c => c.Sensors)
                .FirstOrDefaultAsync(e => e.ControllerKey == controllerKey && e.IsActive);
        }

        public async Task<Sensor?> GetSensorBySensorKeyAsTracking(string sensorKey)
        {
            return await context.Sensors
                .AsTracking()
                .FirstOrDefaultAsync(e => e.SensorKey == sensorKey && e.IsActive);
        }

        public async Task<EdgeDevice?> GetEdgeDeviceByEdgeKeyAsTrackingInactive(
            string edgeKey)
        {
            return await context.EdgeDevices
                .AsTracking()
                .Include(e => e.Controllers)
                    .ThenInclude(c => c.Sensors)
                .FirstOrDefaultAsync(e => e.EdgeKey == edgeKey && !e.IsActive);
        }

        public async Task<Controller?> GetControllerByControllerKeyAsTrackingInactive(
            string controllerKey)
        {
            return await context.Controllers
                .AsTracking()
                .Include(c => c.Sensors)
                .FirstOrDefaultAsync(e => e.ControllerKey == controllerKey && !e.IsActive);
        }

        public async Task<Sensor?> GetSensorBySensorKeyAsTrackingInactive(
            string sensorKey)
        {
            return await context.Sensors
                .AsTracking()
                .FirstOrDefaultAsync(e => e.SensorKey == sensorKey && !e.IsActive);
        }

        public async Task<bool> IsBedAssignedAsync(string edgeKey, string bedNumber)
        {
            var edge = await dbSet
                .Include(e => e.Controllers)
                .FirstOrDefaultAsync(e => e.EdgeKey == edgeKey);

            if (edge == null) return false;

            return edge.Controllers.Any(c => c.BedNumber == bedNumber);
        }

        public async Task<bool> IsRoomAssignedAsync(string roomName)
        {
            return await dbSet.AnyAsync(e => e.RoomName == roomName);
        }

        public void AssignController(Controller controller)
        {
            context.Controllers.Add(controller);
        }

        public void AssignSensor(Sensor sensor)
        {
            context.Sensors.Add(sensor);
        }
        #endregion
    }
}
