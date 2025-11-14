using Application.DTO;
using Application.Interface.IMessagePublisher;
using HCM.MessageBrokerDTOs;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Publisher
{
    public class CreateDeviceProfilePublisher : ICreateDeviceProfilePublisher
    {
        #region Attributes
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateDeviceProfilePublisher> _logger;
        #endregion

        #region Constructor
        public CreateDeviceProfilePublisher(
            IPublishEndpoint publishEndpoint,
            ILogger<CreateDeviceProfilePublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        #endregion

        #region Methods
        public async Task PublishDeviceProfileAsync(CreateDeviceProfileDTO dto)
        {
            _logger.LogInformation(
                "Publishing device profile for patient {PatientIdentityNumber}",
                dto.PatientIdentityNumber);

            // Map nested lists
            var mappedSensors = dto.PatientSensors?.Select(
                s => new HCM.MessageBrokerDTOs.PatientSensorDTO()
                {
                    SensorKey = s.SensorKey,
                    AssignedAt = s.AssignedAt,
                    UnassignedAt = s.UnassignedAt,
                    IsActive = s.IsActive
                }).ToList() ?? new List<HCM.MessageBrokerDTOs.PatientSensorDTO>();

            var mappedStaffs = dto.PatientStaffs?.Select(
                st => new HCM.MessageBrokerDTOs.PatientStaffDTO
                {
                    StaffIdentityNumber = st.StaffIdentityNumber,
                    AssignedAt = st.AssignedAt,
                    UnassignedAt = st.UnassignedAt,
                    IsActive = st.IsActive,
                }).ToList() ?? new List<HCM.MessageBrokerDTOs.PatientStaffDTO>();

            // Map the main DTO
            var message = new DeviceProfileCreate()
            {
                PatientIdentityNumber = dto.PatientIdentityNumber,
                FullName = dto.FullName,
                Dob = dto.Dob,
                Gender = dto.Gender,
                Email = dto.Email,
                Phone = dto.Phone,
                BedAssignedAt = dto.BedAssignedAt,
                BedReleasedAt = dto.BedReleasedAt,
                ControllerKey = dto.ControllerKey,
                BedNumber = dto.BedNumber,
                EdgeKey = dto.EdgeKey,
                RoomName = dto.RoomName,
                IpAddress = dto.IpAddress,
                FirmwareVersion = dto.FirmwareVersion,
                IsActive = dto.IsActive,
                PatientSensors = mappedSensors,
                PatientStaffs = mappedStaffs
            };

            await _publishEndpoint.Publish(message);
        }

        #endregion
    }
}
