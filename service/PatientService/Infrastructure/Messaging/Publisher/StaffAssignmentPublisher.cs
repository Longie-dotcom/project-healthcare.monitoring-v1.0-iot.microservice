using Application.Interface.IMessagePublisher;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Publisher
{
    public class StaffAssignmentPublisher : IStaffAssignmentPublisher
    {
        #region Attributes
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<StaffAssignmentPublisher> _logger;
        #endregion

        #region Constructor
        public StaffAssignmentPublisher(
            IPublishEndpoint publishEndpoint,
            ILogger<StaffAssignmentPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        #endregion

        #region Methods
        public async Task PublishAssignPatientStaff(PatientStaffCreate dto)
        {
            try
            {
                _logger.LogInformation(
                    $"Publishing assign staff for patient {dto.PatientIdentityNumber}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when published assign staff for patient {dto.PatientIdentityNumber}: {ex.Message}");
            }
        }

        public async Task PublishUnassignPatientStaff(PatientStaffRemove dto)
        {
            try
            {
                _logger.LogInformation(
                    $"Publishing unassign staff for patient {dto.PatientIdentityNumber}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when published unassign staff for patient {dto.PatientIdentityNumber}: {ex.Message}");
            }
        }
        #endregion
    }
}
