using Application.DTO;
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

        public async Task PublishAsync(SyncDeleteUserDTO dto)
        {
            _logger.LogInformation($"Publishing user delete message for {dto.IdentityNumber}");
            
            var mappedDto = new DeleteUser()
            { 
                IdentityNumber = dto.IdentityNumber,
                PerformedBy = dto.PerformedBy,
            };

            await _publishEndpoint.Publish(mappedDto);
        }
    }
}
