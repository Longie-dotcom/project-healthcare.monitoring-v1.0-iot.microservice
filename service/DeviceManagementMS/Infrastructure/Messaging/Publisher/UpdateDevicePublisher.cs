using Application.Interface.IMessagePublisher;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Publisher
{
    public class UpdateDevicePublisher : IUpdateDevicePublisher
    {
        #region Attributes
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UpdateDevicePublisher> _logger;
        #endregion

        #region Properties
        #endregion

        public UpdateDevicePublisher(
            IPublishEndpoint publishEndpoint, ILogger<UpdateDevicePublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        #region Methods
        public async Task PublishUpdateEdgeAsync(UpdateEdgeDeviceDTO dto)
        {
            try
            {
                _logger.LogInformation(
                    $"Publish update edge with key: {dto.EdgeKey}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when publish update edge with key: {dto.EdgeKey}, {ex.Message}");
            }
        }

        public async Task PublishUpdateControllerAsync(UpdateControllerDTO dto)
        {
            try
            {
                _logger.LogInformation(
                    $"Publish update controller with key: {dto.ControllerKey} at the edge: {dto.EdgeKey}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when publish update controller with key: {dto.ControllerKey} at the edge: {dto.EdgeKey}, {ex.Message}");
            }
        }

        public async Task PublishUpdateSensorAsync(UpdateSensorDTO dto)
        {
            try
            {
                _logger.LogInformation(
                    $"Publish update sensor with key: {dto.SensorKey} of controller: {dto.ControllerKey} at the edge: {dto.EdgeKey}");
                await _publishEndpoint.Publish(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when publish update sensor with key: {dto.SensorKey} of controller: {dto.ControllerKey} at the edge: {dto.EdgeKey}, {ex.Message}");
            }
        }
        #endregion
    }
}
