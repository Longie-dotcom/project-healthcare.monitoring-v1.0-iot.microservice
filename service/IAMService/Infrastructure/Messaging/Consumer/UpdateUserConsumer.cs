using Application.DTO;
using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class UpdateUserConsumer : IConsumer<UpdateUser>
    {
        private readonly ILogger<UpdateUserConsumer> _logger;
        private readonly IUserService _userService;

        public UpdateUserConsumer(
            ILogger<UpdateUserConsumer> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<UpdateUser> context)
        {
            var dto = context.Message;
            _logger.LogInformation($"Received user updated for {dto.IdentityNumber}");

            // IAM does not accept updated field phone and email from other services
            var mappedDto = new SyncUpdateUserDTO()
            {
                PerformedBy = dto.PerformedBy,
                IdentityNumber = dto.IdentityNumber,
                FullName = dto.Name,
                Address = dto.Address,
                DateOfBirth = dto.Dob,
                Gender = dto.Gender,
            };

            await _userService.SyncUserUpdate(mappedDto);
        }
    }
}
