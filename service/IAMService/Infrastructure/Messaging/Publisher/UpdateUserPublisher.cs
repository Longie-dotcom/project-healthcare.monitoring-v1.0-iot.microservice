using Application.Interface.IMessagePublisher;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MessageBroker.Publisher
{
    public class UpdateUserPublisher : IUpdateUserPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UpdateUserPublisher> _logger;

        public UpdateUserPublisher(
            IPublishEndpoint publishEndpoint, ILogger<UpdateUserPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishAsync(UpdateUser dto)
        {
            try
            {
                _logger.LogInformation($"Publishing user update message for {dto.IdentityNumber}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed when publishing user update message for {dto.IdentityNumber}, {ex.Message}");
            }
        }
    }
}
