using Application.DTO;
using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class UpdateUserConsumer : IConsumer<UpdateUser>
    {
        #region Attributes
        private readonly IRoomProfileService deviceProfileService;
        private readonly ILogger<UpdateUserConsumer> logger;
        #endregion

        #region Properties
        #endregion

        public UpdateUserConsumer(
            IRoomProfileService deviceProfileService,
            ILogger<UpdateUserConsumer> logger)
        {
            this.deviceProfileService = deviceProfileService;
            this.logger = logger;
        }

        #region Methods
        public async Task Consume(ConsumeContext<UpdateUser> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received UpdateUser for identity number: {IdentityNumber}", message.IdentityNumber);

            try
            {
                await deviceProfileService.SyncIamInfoAsync(new UpdateUserInfoDTO
                {
                    Dob = message.Dob,
                    Email = message.Email ?? string.Empty,
                    FullName = message.Name ?? string.Empty,
                    Gender = message.Gender ?? string.Empty,
                    IdentityNumber = message.IdentityNumber,
                    Phone = message.Phone ?? string.Empty
                });
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to sync user for identity number: {IdentityNumber}", message.IdentityNumber);
            }
        }
        #endregion
    }
}
