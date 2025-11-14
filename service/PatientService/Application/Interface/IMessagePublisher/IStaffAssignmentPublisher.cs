using HCM.MessageBrokerDTOs;

namespace Application.Interface.IMessagePublisher
{
    public interface IStaffAssignmentPublisher
    {
        Task PublishAssignPatientStaff(PatientStaffCreate dto);
        Task PublishUnassignPatientStaff(PatientStaffRemove dto);
    }
}
