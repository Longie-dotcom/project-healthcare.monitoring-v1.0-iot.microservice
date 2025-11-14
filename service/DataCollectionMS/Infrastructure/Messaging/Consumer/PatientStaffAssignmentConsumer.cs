using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class PatientStaffAssignmentConsumer : 
        IConsumer<PatientStaffCreate>,
        IConsumer<PatientStaffRemove>
    {
        #region Attributes
        private readonly IRoomProfileService deviceProfileService;
        private readonly ILogger<PatientStaffAssignmentConsumer> logger;
        #endregion

        #region Properties
        #endregion

        public PatientStaffAssignmentConsumer(
            IRoomProfileService deviceProfileService,
            ILogger<PatientStaffAssignmentConsumer> logger)
        {
            this.deviceProfileService = deviceProfileService;
            this.logger = logger;
        }

        #region Methods
        public async Task Consume(ConsumeContext<PatientStaffCreate> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received PatientStaffCreate for StaffIdentityNumber: {StaffIdentityNumber}", message.StaffIdentityNumber);
            try
            {
                await deviceProfileService.AssignPatientStaff(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to assign staff for StaffIdentityNumber: {StaffIdentityNumber}", message.StaffIdentityNumber);
            }
        }

        public async Task Consume(ConsumeContext<PatientStaffRemove> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received PatientStaffRemove for StaffIdentityNumber: {StaffIdentityNumber}", message.StaffIdentityNumber);
            try
            {
                await deviceProfileService.UnassignPatientStaff(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to unassign staff for StaffIdentityNumber: {StaffIdentityNumber}", message.StaffIdentityNumber);
            }
        }
        #endregion
    }
}
