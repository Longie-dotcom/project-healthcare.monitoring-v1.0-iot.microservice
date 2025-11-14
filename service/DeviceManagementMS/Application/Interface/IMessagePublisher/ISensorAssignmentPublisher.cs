using HCM.MessageBrokerDTOs;

namespace Application.Interface.IMessagePublisher
{
    public interface ISensorAssignmentPublisher
    {
        Task AssignSensor(PatientSensorCreate dto);
        Task UnassignSensor(PatientSensorRemove dto);
    }
}
