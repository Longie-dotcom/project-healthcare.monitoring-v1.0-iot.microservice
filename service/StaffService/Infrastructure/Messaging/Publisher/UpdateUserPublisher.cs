using Application.DTO;
using Application.Interface.IMessagePublisher;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Publisher
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

        public async Task PublishAsync(IAMSyncUpdateDTO dto)
        {
            _logger.LogInformation("Publishing IAM update for {IdentityNumber}", dto.IdentityNumber);

            // Email and phone can not be updated from staff service
            var mappedDto = new UpdateUser()
            {
                IdentityNumber = dto.IdentityNumber,
                PerformedBy = dto.PerformedBy,
                Address = dto.Address,
                Dob = dto.DateOfBirth,
                Name = dto.FullName,
                Gender = dto.Gender,
            };

            await _publishEndpoint.Publish(mappedDto);
        }
    }
}
