using Application.DTO;
using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class CreateDeviceProfileConsumer : IConsumer<DeviceProfileCreate>
    {
        #region Attributes
        private readonly IRoomProfileService deviceProfileService;
        private readonly ILogger<CreateDeviceProfileConsumer> logger;
        #endregion

        #region Properties
        #endregion

        public CreateDeviceProfileConsumer(
            IRoomProfileService deviceProfileService,
            ILogger<CreateDeviceProfileConsumer> logger)
        {
            this.deviceProfileService = deviceProfileService;
            this.logger = logger;
        }

        #region Methods
        public async Task Consume(ConsumeContext<DeviceProfileCreate> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received DeviceProfileCreate for ControllerKey: {ControllerKey}", message.ControllerKey);
            try
            {
                await deviceProfileService.CreateDeviceProfile(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to add profile for ControllerKey: {ControllerKey}", message.ControllerKey);
            }
        }
        #endregion
    }
}
