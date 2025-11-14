using Application.DTO;
using HCM.MessageBrokerDTOs;

namespace Application.Interface.IService
{
    public interface IRoomProfileService
    {
        Task<IEnumerable<RoomProfileDTO>> GetDeviceProfilesAsync();

        Task SyncIamInfoAsync(UpdateUser dto);
        Task CreateDeviceProfile(DeviceProfileCreate dto);

        Task SyncEdgeDeviceAsync(UpdateEdgeDeviceDTO dto);
        Task SyncControllerAsync(UpdateControllerDTO dto);
        Task SyncSensorAsync(UpdateSensorDTO dto);
    }
}