using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class CreateRoomProfile : IConsumer<DeviceCreate>
    {
        #region Attributes
        private readonly IRoomProfileService deviceProfileService;
        private readonly ILogger<CreateRoomProfile> logger;
        #endregion

        #region Properties
        #endregion

        public CreateRoomProfile(
            IRoomProfileService deviceProfileService,
            ILogger<CreateRoomProfile> logger)
        {
            this.deviceProfileService = deviceProfileService;
            this.logger = logger;
        }

        #region Methods
        public async Task Consume(ConsumeContext<DeviceCreate> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received DeviceCreate for EdgeKey: {EdgeKey}", message.EdgeKey);
            try
            {
                deviceProfileService.CreateRoomProfile(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to create new room for EdgeKey: {EdgeKey}", message.EdgeKey);
            }
        }
        #endregion
    }
}
