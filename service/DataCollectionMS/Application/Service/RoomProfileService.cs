using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.DomainException;
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
        public async Task<IEnumerable<RoomProfileDTO>> GetRoomProfilesAsync()
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
                try
                {
                    var device = room.GetAssignedDeviceProfileByPatientIdentity(dto.IdentityNumber);

                    device.UpdateIamInfo(dto.Name, dto.Dob, dto.Gender, dto.Email, dto.Phone);

                    repo.Update(room.ID, room);
                }
                catch (InvalidRoomProfileAggregateException)
                {
                    continue;
                }
            }
        }

        public async Task CreateDeviceProfile(DeviceProfileCreate dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();

            // Check if room existence
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with EdgeKey '{dto.EdgeKey}' not found.");

            // Check if patient already exists on active device
            if (room.DeviceProfiles.Any(d => d.IdentityNumber == dto.IdentityNumber && d.UnassignedAt == null))
                throw new PatientExisted($"Patient: {dto.IdentityNumber} already exists on an active device in this room.");

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
        
        public async Task RemoveDeviceProfile(DeviceProfileRemove dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();

            // Check if room existence
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with EdgeKey '{dto.EdgeKey}' not found.");

            room.ReleaseBed(dto.ControllerKey);
            repo.Update(room.ID, room);
        }

        public async Task AssignPatientStaff(PatientStaffCreate dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();

            var rooms = await repo.GetAllAsync();
            foreach (var room in rooms)
            {
                var patient = room.GetAssignedDeviceProfileByPatientIdentity(dto.PatientIdentityNumber);
                if (patient != null)
                {
                    var patientStaff = new PatientStaff(
                        Guid.NewGuid(),
                        dto.PatientIdentityNumber,
                        dto.StaffIdentityNumber,
                        dto.AssignedAt);
                    patient.AssignStaff(patientStaff);
                    repo.Update(room.ID, room);
                    return;
                }
            };

            throw new StaffAssignmentNotFound(
                $"Staff with identity number: {dto.StaffIdentityNumber} " +
                $"of patient: {dto.PatientIdentityNumber} is not found");
        }

        public async Task UnassignPatientStaff(PatientStaffRemove dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();

            var rooms = await repo.GetAllAsync();
            foreach(var room in rooms)
            {
                var staff = room.GetActiveStaff(dto.StaffIdentityNumber);
                if (staff != null && staff.PatientIdentityNumber == dto.PatientIdentityNumber)
                {
                    staff.Unassign();
                    repo.Update(room.ID, room);
                    return;
                }
            };

            throw new StaffAssignmentNotFound(
                $"Staff with identity number: {dto.StaffIdentityNumber} " +
                $"of patient: {dto.PatientIdentityNumber} is not found");
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
                throw new RoomProfileNotFound($"Room with edge key: {dto.EdgeKey} not found.");

            var device = room.GetAssignedDeviceProfileByControllerKey(dto.ControllerKey);
            if (device == null)
                throw new PatientSensorNotFound($"Controller with controller key: {dto.ControllerKey} not found");
            device.UpdateDeviceInfo(dto.IpAddress, dto.BedNumber, dto.IsActive);

            repo.Update(room.ID, room);
        }

        public async Task SyncSensorAsync(UpdateSensorDTO dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with edge key: {dto.EdgeKey} not found.");

            var sensor = room.GetAssignedSensor(dto.ControllerKey, dto.SensorKey);
            if (sensor == null)
                throw new PatientSensorNotFound($"Sensor with sensor key: {dto.SensorKey} not found");
            sensor.UpdateSensorInfo(dto.IsActive);

            repo.Update(room.ID, room);
        }
        #endregion
    }
}
