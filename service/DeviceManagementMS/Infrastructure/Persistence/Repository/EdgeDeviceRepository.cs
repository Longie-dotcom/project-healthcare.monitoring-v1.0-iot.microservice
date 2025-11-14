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

        public void AssignController(Controller controller)
        {
            context.Controllers.Add(controller);
        }

        public void AssignSensor(Sensor sensor)
        {
            context.Sensors.Add(sensor);
        }

        public async Task<bool> IsRoomAssignedAsync(string roomName)
        {
            return await dbSet.AnyAsync(e => e.RoomName == roomName);
        }
        #endregion
    }
}
