using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using Infrastructure.Persistence.Configuration;
using Infrastructure.Persistence.Enum;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repository
{
    public class RoomProfileRepository :
        GenericRepository<RoomProfile>,
        IRoomProfileRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public RoomProfileRepository(DataCollectionDBContext context)
            : base(context, CollectionName.ROOM_PROFILES) { }

        #region Methods
        public async Task<RoomProfile?> GetRoomProfileByKeyAsync(
            string edgeKey)
        {
            return await collection.Find(r => r.EdgeKey == edgeKey).FirstOrDefaultAsync();
        }

        public async Task<RoomProfile?> GetRoomProfileByIPAsync(string edgeIP)
        {
            return await collection.Find(r => r.IpAddress == edgeIP && r.IsActive).FirstOrDefaultAsync();
        }
        #endregion
    }
}
