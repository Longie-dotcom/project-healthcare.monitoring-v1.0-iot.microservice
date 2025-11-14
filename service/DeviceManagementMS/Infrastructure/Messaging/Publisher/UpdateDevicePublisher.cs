using Application.DTO;
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
        public async Task PublishUpdateControllerAsync(ControllerSyncDTO dto)
        {
            _logger.LogInformation(
                $"Publish update controller with key: {dto.ControllerKey}");

            var mappedDto = new UpdateControllerDTO()
            {
                BedNumber = dto.BedNumber,
                ControllerKey = dto.ControllerKey,
                FirmwareVersion = dto.FirmwareVersion,
                IpAddress = dto.IpAddress,
                IsActive = dto.IsActive,
                PerformedBy = dto.PerformedBy,
            };

            await _publishEndpoint.Publish(mappedDto);
        }

        public async Task PublishUpdateEdgeAsync(EdgeDeviceSyncDTO dto)
        {
            _logger.LogInformation(
                $"Publish update controller with key: {dto.ControllerKey}");

            var mappedDto = new UpdateEdgeDeviceDTO()
            {
                ControllerKey = dto.ControllerKey,
                IpAddress = dto.IpAddress,
                IsActive = dto.IsActive,
                PerformedBy = dto.PerformedBy,
                RoomName = dto.RoomName,
            };

            await _publishEndpoint.Publish(mappedDto);
        }

        public async Task PublishUpdateSensorAsync(SensorSyncDTO dto)
        {
            _logger.LogInformation(
                $"Publish update sensor of controller: {dto.ControllerKey} with key: {dto.SensorKey}");

            var mappedDto = new UpdateSensorDTO()
            {
                ControllerKey = dto.ControllerKey,
                IsActive = dto.IsActive,
                SensorKey = dto.SensorKey,
                Description = dto.Description,
                Type = dto.Type,
                Unit = dto.Unit,
                PerformedBy = dto.PerformedBy,
            };

            await _publishEndpoint.Publish(mappedDto);
        }
        #endregion
    }
}
