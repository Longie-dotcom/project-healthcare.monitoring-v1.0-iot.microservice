using Application.Interface.IMessagePublisher;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Publisher
{
    public class CreateRoomPublisher : ICreateRoomPublisher
    {
        #region Attributes
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateRoomPublisher> _logger;
        #endregion

        #region Properties
        #endregion

        public CreateRoomPublisher(
            IPublishEndpoint publishEndpoint, ILogger<CreateRoomPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        #region Methods
        public async Task PublishCreateRoomAsync(DeviceCreate dto)
        {
            try
            {
                _logger.LogInformation(
                    $"Publish create edge device with key: {dto.EdgeKey}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when publish create edge device with key: {dto.EdgeKey}, {ex.Message}");
            }
        }
        #endregion
    }
}
