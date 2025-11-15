using Application.Interface.IMessagePublisher;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Publisher
{
    public class DeviceProfileAssignmentPublisher : IDeviceProfileAssignmentPublisher
    {
        #region Attributes
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<DeviceProfileAssignmentPublisher> _logger;
        #endregion

        #region Constructor
        public DeviceProfileAssignmentPublisher(
            IPublishEndpoint publishEndpoint,
            ILogger<DeviceProfileAssignmentPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        #endregion

        #region Methods
        public async Task PublishAssignDeviceProfile(DeviceProfileCreate dto)
        {
            try
            {
                _logger.LogInformation(
                    $"Publishing assign new device profile for patient {dto.IdentityNumber}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when published assign new device profile for patient {dto.IdentityNumber}: {ex.Message}");
            }
        }

        public async Task PublishUnassignDeviceProfile(DeviceProfileRemove dto)
        {
            try
            {
                _logger.LogInformation(
                    $"Publishing remove device profile for patient {dto.IdentityNumber}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when published remove device profile for patient {dto.IdentityNumber}: {ex.Message}");
            }
        }
        #endregion
    }
}
