using Application.Interface.IService;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class UpdateDeviceProfileConsumer :
        IConsumer<HCM.MessageBrokerDTOs.UpdateEdgeDeviceDTO>,
        IConsumer<HCM.MessageBrokerDTOs.UpdateControllerDTO>,
        IConsumer<HCM.MessageBrokerDTOs.UpdateSensorDTO>
    {
        #region Attributes
        private readonly IRoomProfileService deviceProfileService;
        private readonly ILogger<UpdateDeviceProfileConsumer> logger;
        #endregion

        #region Properties
        #endregion

        public UpdateDeviceProfileConsumer(
            IRoomProfileService deviceProfileService,
            ILogger<UpdateDeviceProfileConsumer> logger)
        {
            this.deviceProfileService = deviceProfileService;
            this.logger = logger;
        }

        #region Methods
        public async Task Consume(
            ConsumeContext<HCM.MessageBrokerDTOs.UpdateEdgeDeviceDTO> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received UpdateEdgeDeviceDTO for EdgeKey: {EdgeKey}", message.EdgeKey);
            try
            {
                await deviceProfileService.SyncEdgeDeviceAsync(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to sync edge device for EdgeKey: {EdgeKey}", message.EdgeKey);
            }
        }

        public async Task Consume(
            ConsumeContext<HCM.MessageBrokerDTOs.UpdateControllerDTO> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received UpdateControllerDTO for ControllerKey: {ControllerKey}", message.ControllerKey);
            try
            {
                // Map to domain DTO
                await deviceProfileService.SyncControllerAsync(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to sync controller for ControllerKey: {ControllerKey}", message.ControllerKey);
            }
        }

        public async Task Consume(
            ConsumeContext<HCM.MessageBrokerDTOs.UpdateSensorDTO> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received UpdateSensorDTO for SensorKey: {SensorKey}", message.SensorKey);
            try
            {
                // Map to domain DTO
                await deviceProfileService.SyncSensorAsync(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to sync sensor for SensorKey: {SensorKey}", message.SensorKey);
            }
        }
        #endregion
    }
}
