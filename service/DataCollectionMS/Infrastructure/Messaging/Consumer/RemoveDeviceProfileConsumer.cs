using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class RemoveDeviceProfileConsumer : IConsumer<DeviceProfileRemove>
    {
        #region Attributes
        private readonly IRoomProfileService deviceProfileService;
        private readonly ILogger<RemoveDeviceProfileConsumer> logger;
        #endregion

        #region Properties
        #endregion

        public RemoveDeviceProfileConsumer(
            IRoomProfileService deviceProfileService,
            ILogger<RemoveDeviceProfileConsumer> logger)
        {
            this.deviceProfileService = deviceProfileService;
            this.logger = logger;
        }

        #region Methods
        public async Task Consume(ConsumeContext<DeviceProfileRemove> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received DeviceProfileRemove for ControllerKey: {ControllerKey}", message.ControllerKey);
            try
            {
                await deviceProfileService.RemoveDeviceProfile(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to remove profile for ControllerKey: {ControllerKey}", message.ControllerKey);
            }
        }
        #endregion
    }
}
