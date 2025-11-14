using Domain.Aggregate;

namespace Domain.IRepository
{
    public interface IRoomProfileRepository :
        IGenericRepository<RoomProfile>,
        IRepositoryBase
    {
        Task<RoomProfile?> GetRoomProfileByKeyAsync(string edgeKey);
    }
}
