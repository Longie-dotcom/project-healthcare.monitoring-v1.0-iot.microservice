using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.Entity;
using Domain.IRepository;
using HCM.MessageBrokerDTOs;

namespace Application.Service
{
    public class RoomProfileService : IRoomProfileService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public RoomProfileService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<IEnumerable<RoomProfileDTO>> GetDeviceProfilesAsync()
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var rooms = await repo.GetAllAsync();
            var devices = rooms.SelectMany(r => r.DeviceProfiles);
            return mapper.Map<IEnumerable<RoomProfileDTO>>(devices);
        }

        public async Task SyncIamInfoAsync(UpdateUser dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var rooms = await repo.GetAllAsync();

            foreach (var room in rooms)
            {
                var device = room.GetAssignedDeviceProfileByPatientIdentity(dto.IdentityNumber);
                device.UpdateIamInfo(dto.Name, dto.Dob, dto.Gender, dto.Email, dto.Phone);
                repo.Update(room.ID, room);
            }
        }

        public async Task CreateDeviceProfile(DeviceProfileCreate dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();

            // Check if room exists
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with EdgeKey '{dto.EdgeKey}' not found.");

            // Check if patient already exists on active device
            if (room.DeviceProfiles.Any(d => d.IdentityNumber == dto.IdentityNumber && d.IsActive))
                throw new PatientExisted(dto.IdentityNumber);

            var device = new DeviceProfile(
                Guid.NewGuid(),
                dto.IdentityNumber,
                dto.FullName,
                dto.Dob,
                dto.Gender,
                dto.Email,
                dto.Phone,
                dto.AssignedAt ?? DateTime.UtcNow,
                dto.EdgeKey,
                dto.ControllerKey,
                dto.IpAddress,
                dto.BedNumber
            );

            room.AssignBed(device);
            repo.Update(room.ID, room);
        }

        public async Task SyncEdgeDeviceAsync(UpdateEdgeDeviceDTO dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with EdgeKey '{dto.EdgeKey}' not found.");

            room.UpdateIamInfo(dto.IpAddress, dto.RoomName, dto.IsActive);
            repo.Update(room.ID, room);
        }

        public async Task SyncControllerAsync(UpdateControllerDTO dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with EdgeKey '{dto.EdgeKey}' not found.");

            var device = room.GetAssignedDeviceProfileByControllerKey(dto.ControllerKey);
            device.UpdateDeviceInfo(dto.IpAddress, dto.BedNumber, dto.IsActive);

            repo.Update(room.ID, room);
        }

        public async Task SyncSensorAsync(UpdateSensorDTO dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with EdgeKey '{dto.EdgeKey}' not found.");

            var sensor = room.GetAssignedSensor(dto.ControllerKey, dto.SensorKey);
            sensor.UpdateSensorInfo(dto.IsActive);

            repo.Update(room.ID, room);
        }
        #endregion
    }
}
