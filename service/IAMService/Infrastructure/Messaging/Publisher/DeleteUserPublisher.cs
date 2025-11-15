using Application.Interface.IMessagePublisher;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MessageBroker.Publisher
{
    public class DeleteUserPublisher : IDeleteUserPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<DeleteUserPublisher> _logger;

        public DeleteUserPublisher(IPublishEndpoint publishEndpoint, ILogger<DeleteUserPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishAsync(DeleteUser dto)
        {
            try
            {
                _logger.LogInformation($"Publishing user delete message for {dto.IdentityNumber}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed when publishing user delete message for {dto.IdentityNumber}, {ex.Message}");
            }
        }
    }
}
