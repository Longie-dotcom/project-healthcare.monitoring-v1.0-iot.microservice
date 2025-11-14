using Application.DTO;

namespace Application.IGrpc
{
    public interface IDeviceManagementGrpcClient
    {
        Task<DeviceProfileControllerDTO> GetControllerMetaAsync(string controllerKey);
    }
}
