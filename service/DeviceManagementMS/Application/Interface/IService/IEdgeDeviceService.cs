using Application.DTO;

namespace Application.Interface.IService
{
    public interface IEdgeDeviceService
    {
        // Edge
        Task<EdgeDeviceDTO> GetByIdAsync(Guid edgeDeviceId);
        Task<IEnumerable<EdgeDeviceDTO>> GetAllAsync(QueryEdgeDeviceDTO dto, string sort);
        Task<EdgeDeviceDTO> CreateAsync(EdgeDeviceCreateDTO dto);
        Task<EdgeDeviceDTO> UpdateAsync(Guid edgeDeviceId, EdgeDeviceUpdateDTO dto);
        Task DeactiveAsync(Guid edgeDeviceId, EdgeDeviceDeactiveDTO dto);


        // Controller
        Task AssignControllerAsync(ControllerCreateDTO dto);
        Task<ControllerDTO> UpdateControllerAsync(ControllerUpdateDTO dto);
        Task UnassignControllerAsync(ControllerUnassignDTO dto);

        // Sensor
        Task AssignSensorAsync(SensorCreateDTO dto);
        Task<SensorDTO> UpdateSensorAsync(SensorUpdateDTO dto);
        Task UnassignSensorAsync(SensorUnassignDTO dto);

        Task<DeviceProfileControllerDTO> GetControllerMetaAsync(string controllerKey);
    }
}
