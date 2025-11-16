using Application.Interface.IService;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class PatientSensorAssignmentConsumer :
        IConsumer<PatientSensorCreate>,
        IConsumer<PatientSensorRemove>
    {
        #region Attributes
        private readonly IRoomProfileService deviceProfileService;
        private readonly ILogger<PatientSensorAssignmentConsumer> logger;
        #endregion

        #region Properties
        #endregion

        public PatientSensorAssignmentConsumer(
            IRoomProfileService deviceProfileService,
            ILogger<PatientSensorAssignmentConsumer> logger)
        {
            this.deviceProfileService = deviceProfileService;
            this.logger = logger;
        }

        #region Methods
        public async Task Consume(ConsumeContext<PatientSensorCreate> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received PatientSensorCreate for SensorKey: {SensorKey}", message.SensorKey);
            try
            {
                await deviceProfileService.AssignPatientSensor(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to assign staff for SensorKey: {SensorKey}", message.SensorKey);
            }
        }

        public async Task Consume(ConsumeContext<PatientSensorRemove> context)
        {
            var message = context.Message;
            logger.LogInformation(
                "Received PatientSensorRemove for SensorKey: {SensorKey}", message.SensorKey);
            try
            {
                await deviceProfileService.UnassignPatientSensor(message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, "Failed to unassign sensor for SensorKey: {SensorKey}", message.SensorKey);
            }
        }
        #endregion
    }
}
