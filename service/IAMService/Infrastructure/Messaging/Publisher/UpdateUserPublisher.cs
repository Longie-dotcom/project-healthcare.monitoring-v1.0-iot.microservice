using Application.DTO;
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

        public async Task PublishAsync(SyncUpdateUserDTO dto)
        {
            _logger.LogInformation($"Publishing user update message for {dto.IdentityNumber}");

            var mappedDto = new UpdateUser()
            { 
                IdentityNumber = dto.IdentityNumber,
                PerformedBy = dto.PerformedBy,
                Email = dto.Email,
                Address = dto.Address,
                Dob = dto.DateOfBirth,
                Name = dto.FullName,
                Gender = dto.Gender,
                Phone = dto.Phone,
            };

            await _publishEndpoint.Publish(mappedDto);
        }
    }
}
