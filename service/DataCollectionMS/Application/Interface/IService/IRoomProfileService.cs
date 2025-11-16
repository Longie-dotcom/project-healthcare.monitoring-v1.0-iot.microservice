using Application.DTO;
using HCM.MessageBrokerDTOs;

namespace Application.Interface.IService
{
    public interface IRoomProfileService
    {
        Task<IEnumerable<RoomProfileDTO>> GetRoomProfilesAsync();

        void CreateRoomProfile(DeviceCreate dto);
        Task ReceiveDataAsync(RawSensorData dto);

        Task SyncIamInfoAsync(UpdateUser dto);
        Task CreateDeviceProfile(DeviceProfileCreate dto);
        Task RemoveDeviceProfile(DeviceProfileRemove dto);

        Task AssignPatientStaff(PatientStaffCreate dto);
        Task UnassignPatientStaff(PatientStaffRemove dto);

        Task AssignPatientSensor(PatientSensorCreate dto);
        Task UnassignPatientSensor(PatientSensorRemove dto);

        Task SyncEdgeDeviceAsync(UpdateEdgeDeviceDTO dto);
        Task SyncControllerAsync(UpdateControllerDTO dto);
        Task SyncSensorAsync(UpdateSensorDTO dto);
    }
}