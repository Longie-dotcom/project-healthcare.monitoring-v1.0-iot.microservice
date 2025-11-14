using Application.Interface.IMessagePublisher;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Publisher
{
    public class SensorAssignmentPublisher : ISensorAssignmentPublisher
    {
        #region Attributes
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<SensorAssignmentPublisher> _logger;
        #endregion

        #region Properties
        #endregion

        public SensorAssignmentPublisher(
            IPublishEndpoint publishEndpoint, ILogger<SensorAssignmentPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        #region Methods
        public async Task AssignSensor(PatientSensorCreate dto)
        {
            _logger.LogInformation(
                $"Publish create sensor with key: {dto.SensorKey} of controller: {dto.ControllerKey} in edge: {dto.EdgeKey}");
            await _publishEndpoint.Publish(dto);
        }

        public async Task UnassignSensor(PatientSensorRemove dto)
        {
            _logger.LogInformation(
                $"Publish remove sensor with key: {dto.SensorKey} of controller: {dto.ControllerKey} in edge: {dto.EdgeKey}");
            await _publishEndpoint.Publish(dto);
        }
        #endregion
    }
}
