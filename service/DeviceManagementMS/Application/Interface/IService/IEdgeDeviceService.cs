using Application.DTO;

namespace Application.Interface.IService
{
    public interface IEdgeDeviceService
    {
        Task<EdgeDeviceDTO> GetByIdAsync(Guid edgeDeviceId);
        Task<IEnumerable<EdgeDeviceDTO>> GetAllAsync(QueryEdgeDeviceDTO dto, string sort);
        Task<EdgeDeviceDTO> CreateAsync(EdgeDeviceCreateDTO dto);
        Task<EdgeDeviceDTO> UpdateAsync(Guid edgeDeviceId, EdgeDeviceUpdateDTO dto);
        Task DeleteAsync(Guid edgeDeviceId, EdgeDeviceDeleteDTO dto);

        Task AssignControllerAsync(ControllerCreateDTO dto);
        Task AssignSensorAsync(SensorCreateDTO dto);
        Task UnassignControllerAsync(ControllerDeleteDTO dto);
        Task UnassignSensorAsync(SensorDeleteDTO dto);

        Task ReactivateEdgeDeviceAsync(string edgeKey, string performedBy);
        Task ReactivateControllerAsync(string controllerKey, string performedBy);
        Task ReactivateSensorAsync(string sensorKey, string performedBy);

        Task<DeviceProfileControllerDTO> GetControllerMetaAsync(string controllerKey);
    }
}
