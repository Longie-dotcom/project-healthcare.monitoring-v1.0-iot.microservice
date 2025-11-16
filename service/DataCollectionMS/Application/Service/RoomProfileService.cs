using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IMessageBrokerPublisher;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.DomainException;
using Domain.Entity;
using Domain.IRepository;
using Domain.ValueObject;
using HCM.MessageBrokerDTOs;

namespace Application.Service
{
    public class RoomProfileService : IRoomProfileService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ISignalREnvelopePublisher signalREnvelopePublisher;
        #endregion

        #region Properties
        #endregion

        public RoomProfileService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ISignalREnvelopePublisher signalREnvelopePublisher)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.signalREnvelopePublisher = signalREnvelopePublisher;
        }

        #region Methods
        public async Task<IEnumerable<RoomProfileDTO>> GetRoomProfilesAsync()
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var rooms = await repo.GetAllAsync();
            return mapper.Map<IEnumerable<RoomProfileDTO>>(rooms);
        }

        public void CreateRoomProfile(DeviceCreate dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var room = new RoomProfile(
                Guid.NewGuid(),
                dto.EdgeKey,
                dto.IpAddress ?? "",
                dto.RoomName ?? "");
            repo.Add(room);
        }

        public async Task ReceiveDataAsync(RawSensorData dto)
        {
            if (dto.SensorDatas == null || !dto.SensorDatas.Any())
                return; // nothing to process

            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var room = await repo.GetRoomProfileByIPAsync(dto.EdgeIP);

            if (room != null)
            {
                var controller = room.GetAssignedDeviceProfileByIP(dto.ControllerIP);

                if (controller != null && dto.SensorDatas != null && dto.SensorDatas.Any())
                {
                    // Add data
                    var listSensor = UpdateControllerSensors(controller, dto.SensorDatas);

                    // Persist the changes
                    repo.Update(room.ID, room);

                    // Publish data to dashboard realtime
                    foreach (var sensor in listSensor)
                    {
                        await ForwardSensorUpdateAsync(
                            controller, 
                            sensor.Sensor, 
                            new SensorValue() 
                            { 
                                DataType = sensor.DataType, 
                                DataValue = sensor.DataValue 
                            });
                    }

                    // Stop once we find the controller
                    return;
                }

            } 
            else
            {
                // Do later: Publish notification that edge device is vulnerable
                Console.WriteLine(
                    $"VULNERABILITY: EdgeIP '{dto.EdgeIP}' not found, using fallback by ControllerIP '{dto.ControllerIP}'");

                // Fallback: Edge device not found by EdgeIP
                // Loop all rooms and all controllers
                var allRooms = await repo.GetAllAsync(); 

                foreach (var r in allRooms)
                {
                    var controller = r.GetAssignedDeviceProfileByIP(dto.ControllerIP);
                    if (controller != null && dto.SensorDatas != null && dto.SensorDatas.Any())
                    {
                        // Add data
                        var listSensor = UpdateControllerSensors(controller, dto.SensorDatas);

                        // Persist the changes
                        repo.Update(r.ID, r);

                        // Publish data to dashboard realtime
                        foreach (var sensor in listSensor)
                        {
                            await ForwardSensorUpdateAsync(
                                controller,
                                sensor.Sensor,
                                new SensorValue()
                                {
                                    DataType = sensor.DataType,
                                    DataValue = sensor.DataValue
                                });
                        }

                        // Stop once we find the controller
                        return;
                    }
                }
            }

            // Do later: Publish notification that device connection is vulnerable
            Console.WriteLine(
                $"VULNERABILITY: EdgeIP '{dto.EdgeIP}' not found, ControllerIP '{dto.ControllerIP}' not found");
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
                throw new PatientExisted(
                    $"Patient: {dto.IdentityNumber} already exists on an active device in this room.");

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

            foreach (var sensorDto in dto.PatientSensors)
            {
                if (sensorDto != null)
                    device.AssignSensor(new PatientSensor(
                        Guid.NewGuid(),
                        dto.ControllerKey,
                        sensorDto.SensorKey,
                        sensorDto.AssignedAt));
            }

            foreach (var staffDto in dto.PatientStaffs)
            {
                if (staffDto != null)
                    device.AssignStaff(new PatientStaff(
                        Guid.NewGuid(),
                        dto.IdentityNumber,
                        staffDto.StaffIdentityNumber,
                        staffDto.AssignedAt));
            }

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

        public async Task AssignPatientSensor(PatientSensorCreate dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with EdgeKey '{dto.EdgeKey}' not found.");

            var device = room.GetAssignedDeviceProfileByControllerKey(dto.ControllerKey);
            if (device == null)
                throw new PatientSensorNotFound($"Controller with controller key: {dto.ControllerKey} not found");

            var sensor = new PatientSensor(
                Guid.NewGuid(),
                dto.ControllerKey,
                dto.SensorKey,
                DateTime.UtcNow);
            device.AssignSensor(sensor);
            repo.Update(room.ID, room);
        }

        public async Task UnassignPatientSensor(PatientSensorRemove dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with EdgeKey '{dto.EdgeKey}' not found.");

            var device = room.GetAssignedDeviceProfileByControllerKey(dto.ControllerKey);
            if (device == null)
                throw new PatientSensorNotFound($"Controller with controller key: {dto.ControllerKey} not found");

            device.UnassignSensor(dto.SensorKey);
            repo.Update(room.ID, room);
        }

        public async Task SyncEdgeDeviceAsync(UpdateEdgeDeviceDTO dto)
        {
            var repo = unitOfWork.GetRepository<IRoomProfileRepository>();
            var room = await repo.GetRoomProfileByKeyAsync(dto.EdgeKey);
            if (room == null)
                throw new RoomProfileNotFound($"Room with EdgeKey '{dto.EdgeKey}' not found.");

            room.UpdateInfo(dto.IpAddress, dto.RoomName, dto.IsActive);
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

        #region Private Helpers
        private IEnumerable<(PatientSensor Sensor, string DataType, object DataValue)> UpdateControllerSensors(
            DeviceProfile controller, 
            List<SensorValue> sensorDatas)
        {
            var sensorValues = sensorDatas
                .Select(sv => (sv.SensorName, sv.DataType, sv.DataValue))
                .ToList();

            return controller.AddMultipleSensorDataByName(sensorValues);
        }

        private async Task ForwardSensorUpdateAsync(
            DeviceProfile controller,
            PatientSensor sensor,
            SensorValue value)
        {
            // Get assigned staff for this controller
            var staffList = controller.GetActiveStaffs();
            if (!staffList.Any())
                return;

            // Create payload to send through SignalR
            var payload = new SensorUpdatePayloadDto()
            {
                PatientIdentityNumber = controller.IdentityNumber,
                ControllerId = controller.DeviceProfileID,
                BedNumber = controller.BedNumber,
                SensorKey = sensor.SensorKey,
                DataType = value.DataType,
                DataValue = value.DataValue,
            };

            // Publish to each staff
            foreach (var staff in staffList)
            {
                var envelope = new SignalREnvelope.SignalREnvelope
                {
                    Topic = staff.StaffIdentityNumber, // SignalR group
                    Method = "Receive",
                    SourceService = "DataCollectionMS",
                    Payload = payload,
                    Timestamp = DateTime.UtcNow
                };

                await signalREnvelopePublisher.PublishAsync(envelope);
            }
        }
        #endregion
    }
}
