using HCM.MessageBrokerDTOs;

namespace Application.Interface.IMessagePublisher
{
    public interface IDeviceProfileAssignmentPublisher
    {
        Task PublishAssignDeviceProfile(DeviceProfileCreate dto);
        Task PublishUnassignDeviceProfile(DeviceProfileRemove dto);
    }
}
