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
            try
            {
                _logger.LogInformation($"Received user updated for {dto.IdentityNumber}");
                await _userService.SyncUserUpdate(context.Message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed when received user updated for {dto.IdentityNumber}, {ex.Message}");
            }
        }
    }
}
