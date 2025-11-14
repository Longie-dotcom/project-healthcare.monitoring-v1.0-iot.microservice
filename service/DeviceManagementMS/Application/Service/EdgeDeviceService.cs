using Application.ApplicationException;
using Application.DTO;
using Application.Helper;
using Application.Interface.IGrpc;
using Application.Interface.IMessagePublisher;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using HCM.MessageBrokerDTOs;

namespace Application.Service
{
    public class EdgeDeviceService : IEdgeDeviceService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUpdateDevicePublisher updateDevicePublisher;
        private readonly IPatientGrpcClient patientGrpcClient;
        private readonly ISensorAssignmentPublisher sensorAssignmentPublisher;

        public EdgeDeviceService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUpdateDevicePublisher updateDevicePublisher,
            IPatientGrpcClient patientGrpcClient,
            ISensorAssignmentPublisher sensorAssignmentPublisher)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.updateDevicePublisher = updateDevicePublisher;
            this.patientGrpcClient = patientGrpcClient;
            this.sensorAssignmentPublisher = sensorAssignmentPublisher;
        }

        #region Methods
        public async Task<EdgeDeviceDTO> GetByIdAsync(Guid edgeDeviceId)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();
            var device = await repo.GetDetailedByIdAsync(edgeDeviceId);
            if (device == null)
                throw new DeviceNotFound($"EdgeDevice {edgeDeviceId} not found");

            return mapper.Map<EdgeDeviceDTO>(device);
        }

        public async Task<IEnumerable<EdgeDeviceDTO>> GetAllAsync(QueryEdgeDeviceDTO dto, string sort)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();
            var devices = await repo.GetAllWithFilter(dto.Search ?? string.Empty, dto.PageIndex, dto.PageLength);

            if (devices == null || !devices.Any())
                throw new DeviceNotFound("The device list is empty");

            return mapper.Map<IEnumerable<EdgeDeviceDTO>>(devices);
        }

        public async Task<EdgeDeviceDTO> CreateAsync(EdgeDeviceCreateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // check room occupied
            var roomAssigned = await repo.IsRoomAssignedAsync(dto.RoomName);
            if (roomAssigned)
                throw new RoomOccupied($"The room: {dto.RoomName} has been occupied");

            // compute next index for key generation (count existing devices)
            var existing = (await repo.GetAllWithFilter(string.Empty, 1, int.MaxValue)).ToList();
            var nextIndex = existing.Count + 1;
            var edgeKey = KeyGenerator.GenerateKey("EDGE", nextIndex);

            var device = new EdgeDevice(
                Guid.NewGuid(),
                edgeKey,
                dto.RoomName,
                dto.IpAddress,
                dto.Description ?? string.Empty
            );

            repo.Add(device);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // publish
            await PublishEdgeDeviceUpdateAsync(device, dto.PerformedBy ?? string.Empty);

            return mapper.Map<EdgeDeviceDTO>(device);
        }

        public async Task<EdgeDeviceDTO> UpdateAsync(Guid edgeDeviceId, EdgeDeviceUpdateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();
            var device = await repo.GetDetailedByIdAsync(edgeDeviceId);
            if (device == null)
                throw new DeviceNotFound($"EdgeDevice {edgeDeviceId} not found");

            // Update fields with validations done in aggregate
            if (!string.IsNullOrWhiteSpace(dto.RoomName) && dto.RoomName != device.RoomName)
            {
                var roomAssigned = await repo.IsRoomAssignedAsync(dto.RoomName);
                if (roomAssigned)
                    throw new RoomOccupied($"The room: {dto.RoomName} has been occupied");

                device.UpdateRoomName(dto.RoomName);
            }

            if (!string.IsNullOrWhiteSpace(dto.IpAddress))
                device.UpdateIpAddress(dto.IpAddress);

            if (!string.IsNullOrWhiteSpace(dto.Description))
                device.UpdateDescription(dto.Description);

            await unitOfWork.CommitAsync(dto.PerformedBy);

            // publish
            await PublishEdgeDeviceUpdateAsync(device, dto.PerformedBy ?? string.Empty);

            return mapper.Map<EdgeDeviceDTO>(device);
        }

        public async Task DeactiveAsync(Guid edgeDeviceId, EdgeDeviceDeactiveDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();
            var device = await repo.GetDetailedByIdAsync(edgeDeviceId);
            if (device == null)
                throw new DeviceNotFound($"EdgeDevice: {edgeDeviceId} not found");

            device.Deactivate();
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // publish to update
            await PublishEdgeDeviceUpdateAsync(device, dto.PerformedBy ?? string.Empty);
        }

        public async Task AssignControllerAsync(ControllerCreateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // fetch owner with children
            var edgeOwner = await repo.GetEdgeDeviceByEdgeKeyAsTracking(dto.EdgeKey);
            if (edgeOwner == null)
                throw new DeviceNotFound($"Edge device with key:{dto.EdgeKey} is not found");

            // validate bed conflict inside the same edge
            var bedNumber = dto.BedNumber ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(bedNumber))
            {
                var bedConflict = edgeOwner.Controllers.Any(c => c.BedNumber == bedNumber && c.IsActive);
                if (bedConflict)
                    throw new BedOccupied($"The bed: {bedNumber} has been occupied in edge {dto.EdgeKey}");
            }

            // validate ip conflict inside the same edge
            var ipConflict = edgeOwner.Controllers.Any(c => c.IpAddress == dto.IpAddress);
            if (ipConflict)
                throw new IPAddressConflicting($"The IP: {dto.IpAddress} has been occupied in edge {dto.EdgeKey}");

            var nextIndex = edgeOwner.Controllers.Count + 1;
            var controllerKey = KeyGenerator.GenerateKey("CTRL", nextIndex);

            var controller = new Controller(
                Guid.NewGuid(),
                controllerKey,
                dto.EdgeKey,
                dto.BedNumber ?? string.Empty,
                dto.IpAddress,
                dto.FirmwareVersion ?? string.Empty
            );

            // domain
            edgeOwner.AddController(controller);

            // persistence helper
            repo.AssignController(controller);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // publish controller: patient ms will publish the using controller only
        }

        public async Task<ControllerDTO> UpdateControllerAsync(ControllerUpdateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // fetch parent edge (we require edgeKey in DTO)
            var edge = await repo.GetEdgeDeviceByEdgeKeyAsTracking(dto.EdgeKey);
            if (edge == null)
                throw new DeviceNotFound($"Edge device with key:{dto.EdgeKey} not found");

            var controller = edge.Controllers.FirstOrDefault(c => c.ControllerKey == dto.ControllerKey);
            if (controller == null)
                throw new DeviceNotFound($"Controller with key:{dto.ControllerKey} not found");

            if (!string.IsNullOrWhiteSpace(dto.BedNumber))
                controller.UpdateBedNumber(dto.BedNumber);

            if (!string.IsNullOrWhiteSpace(dto.IpAddress))
                controller.UpdateIpAddress(dto.IpAddress);

            if (!string.IsNullOrWhiteSpace(dto.FirmwareVersion))
                controller.UpdateFirmwareVersion(dto.FirmwareVersion);

            if (dto.IsActive.HasValue)
            {
                if (dto.IsActive.Value == true)
                {
                    controller.Activate();
                }
                else
                {
                    controller.Deactivate();
                }
            }

            await unitOfWork.CommitAsync(dto.PerformedBy);

            // publish controller (and its sensors)
            await PublishControllerUpdateAsync(controller, dto.PerformedBy ?? string.Empty);

            return mapper.Map<ControllerDTO>(controller);
        }

        public async Task UnassignControllerAsync(ControllerUnassignDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // check with Patient MS if controller is in use
            var inUse = await patientGrpcClient.IsControllerInUseAsync(dto.ControllerKey);
            if (inUse)
                throw new BedInUse($"Controller/beds '{dto.ControllerKey}' is currently in use by patient.");

            // fetch parent edge as tracking
            var edge = await repo.GetEdgeDeviceByEdgeKeyAsTracking(dto.EdgeKey);
            if (edge == null)
                throw new DeviceNotFound($"Edge device with key:{dto.EdgeKey} not found");

            // remove controller, patient will never can call this controller to assign
            edge.RemoveController(dto.ControllerKey);

            await unitOfWork.CommitAsync(dto.PerformedBy);

            // publish controller: patient ms will publish the using controller only
        }

        public async Task AssignSensorAsync(SensorCreateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // fetch edge + controllers as tracking
            var edgeOwner = await repo.GetEdgeDeviceByEdgeKeyAsTracking(dto.EdgeKey);
            if (edgeOwner == null)
                throw new DeviceNotFound($"Edge device with key:{dto.EdgeKey} is not found");

            var controller = edgeOwner.Controllers.FirstOrDefault(c => c.ControllerKey == dto.ControllerKey);
            if (controller == null)
                throw new DeviceNotFound($"Controller device with key:{dto.ControllerKey} is not found");

            // bed/sensor collision is validated in aggregate AddSensor (controller-level)
            var nextIndex = controller.Sensors.Count + 1;
            var sensorKey = KeyGenerator.GenerateKey("SENS", nextIndex);

            var sensor = new Sensor(
                Guid.NewGuid(),
                sensorKey,
                dto.ControllerKey,
                dto.Type,
                dto.Unit,
                dto.Description ?? string.Empty
            );

            controller.AddSensor(sensor);

            // persistence
            repo.AssignSensor(sensor);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // publish
            await PublishSensorAssign(sensor, edgeOwner.EdgeKey, dto.PerformedBy ?? string.Empty);
        }

        public async Task<SensorDTO> UpdateSensorAsync(SensorUpdateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // fetch parent edge which includes controllers + sensors
            var edge = await repo.GetEdgeDeviceByEdgeKeyAsTracking(dto.EdgeKey);
            if (edge == null)
                throw new DeviceNotFound($"Edge device with key:{dto.EdgeKey} not found");

            var controller = edge.Controllers.FirstOrDefault(c => c.ControllerKey == dto.ControllerKey);
            if (controller == null)
                throw new DeviceNotFound($"Controller with key:{dto.ControllerKey} not found");

            var sensor = controller.Sensors.FirstOrDefault(s => s.SensorKey == dto.SensorKey);
            if (sensor == null)
                throw new DeviceNotFound($"Sensor with key:{dto.SensorKey} not found");

            // update fields
            if (!string.IsNullOrWhiteSpace(dto.Type))
                sensor.UpdateType(dto.Type);

            if (!string.IsNullOrWhiteSpace(dto.Unit))
                sensor.UpdateUnit(dto.Unit);

            if (!string.IsNullOrWhiteSpace(dto.Description))
                sensor.UpdateDescription(dto.Description);

            if (dto.IsActive.HasValue)
            {
                if (dto.IsActive.Value == true)
                {
                    controller.Activate();
                }
                else
                {
                    controller.Deactivate();
                }
            }

            await unitOfWork.CommitAsync(dto.PerformedBy);

            // publish change
            await PublishSensorUpdateAsync(sensor, edge.EdgeKey, dto.PerformedBy ?? string.Empty);

            return mapper.Map<SensorDTO>(sensor);
        }

        public async Task UnassignSensorAsync(SensorUnassignDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // fetch controller by searching edge first
            var edge = await repo.GetEdgeDeviceByEdgeKeyAsTracking(dto.EdgeKey);
            if (edge == null)
                throw new DeviceNotFound($"Edge device with key:{dto.EdgeKey} not found");

            var controller = edge.Controllers.FirstOrDefault(c => c.ControllerKey == dto.ControllerKey);
            if (controller == null)
                throw new DeviceNotFound($"Controller with key:{dto.ControllerKey} not found");

            var sensor = controller.Sensors.FirstOrDefault(s => s.SensorKey == dto.SensorKey);
            if (sensor == null)
                throw new DeviceNotFound($"Sensor with key:{dto.SensorKey} not found");

            // remove sensor
            controller.RemoveSensor(dto.SensorKey);

            await unitOfWork.CommitAsync(dto.PerformedBy);

            // publish sensor update
            await PublishSensorUnassign(sensor, edge.EdgeKey, dto.PerformedBy ?? string.Empty, DateTime.UtcNow);
        }

        public async Task<DeviceProfileControllerDTO> GetControllerMetaAsync(string controllerKey)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // Strategy: iterate all edges, fetch each including controllers (uses GetEdgeDeviceByEdgeKey), find the controller
            // This avoids needing a specialized repo method while ensuring controllers are included.
            var edges = (await repo.GetAllWithFilter(string.Empty, 1, int.MaxValue)).ToList();
            foreach (var e in edges)
            {
                // fetch the full edge with includes
                var fullEdge = await repo.GetEdgeDeviceByEdgeKey(e.EdgeKey);
                if (fullEdge == null) continue;

                var controller = fullEdge.Controllers.FirstOrDefault(c => c.ControllerKey == controllerKey);
                if (controller != null)
                {
                    return new DeviceProfileControllerDTO
                    {
                        ControllerKey = controller.ControllerKey,
                        BedNumber = controller.BedNumber,
                        FirmwareVersion = controller.FirmwareVersion,
                        IsActive = controller.IsActive,
                        Status = controller.Status,

                        EdgeKey = fullEdge.EdgeKey,
                        RoomName = fullEdge.RoomName,
                        IpAddress = fullEdge.IpAddress,
                        Description = fullEdge.Description,

                        Sensors = controller.Sensors.Select(s => new DeviceProfileSensorDTO
                        {
                            SensorKey = s.SensorKey,
                            Type = s.Type,
                            Unit = s.Unit,
                            Description = s.Description,
                            IsActive = s.IsActive
                        }).ToList()
                    };
                }
            }

            throw new DeviceNotFound($"Controller '{controllerKey}' not found");
        }
        #endregion

        #region Private Helpers
        private async Task PublishEdgeDeviceUpdateAsync(EdgeDevice device, string performedBy)
        {
            var syncDto = new UpdateEdgeDeviceDTO
            {
                EdgeKey = device.EdgeKey,
                IpAddress = device.IpAddress,
                RoomName = device.RoomName,
                IsActive = device.IsActive,
                PerformedBy = performedBy
            };
            await updateDevicePublisher.PublishUpdateEdgeAsync(syncDto);
        }

        private async Task PublishControllerUpdateAsync(Controller controller, string performedBy)
        {
            // Publish the controller first
            var controllerDto = new UpdateControllerDTO
            {
                EdgeKey = controller.EdgeKey,
                ControllerKey = controller.ControllerKey,
                BedNumber = controller.BedNumber,
                IpAddress = controller.IpAddress,
                IsActive = controller.IsActive,
                PerformedBy = performedBy
            };
            await updateDevicePublisher.PublishUpdateControllerAsync(controllerDto);

            // Publish all child sensors
            foreach (var sensor in controller.Sensors)
            {
                await PublishSensorUpdateAsync(sensor, controller.EdgeKey, performedBy);
            }
        }

        private async Task PublishSensorUpdateAsync(Sensor sensor, string edgeKey, string performedBy)
        {
            var sensorDto = new UpdateSensorDTO
            {
                EdgeKey = edgeKey,
                ControllerKey = sensor.ControllerKey,
                SensorKey = sensor.SensorKey,
                IsActive = sensor.IsActive,
                PerformedBy = performedBy
            };
            await updateDevicePublisher.PublishUpdateSensorAsync(sensorDto);
        }

        private async Task PublishSensorAssign(Sensor sensor, string edgeKey, string performedBy)
        {
            var sensorDto = new PatientSensorCreate()
            {
                EdgeKey = edgeKey,
                ControllerKey = sensor.ControllerKey,
                SensorKey = sensor.SensorKey,
                PerformedBy = performedBy
            };
            await sensorAssignmentPublisher.AssignSensor(sensorDto);
        }

        private async Task PublishSensorUnassign(Sensor sensor, string edgeKey, string performedBy, DateTime unassignedAt)
        {
            var sensorDto = new PatientSensorRemove()
            {
                EdgeKey = edgeKey,
                ControllerKey = sensor.ControllerKey,
                SensorKey = sensor.SensorKey,
                UnassignedAt = unassignedAt,
                PerformedBy = performedBy
            };
            await sensorAssignmentPublisher.UnassignSensor(sensorDto);
        }
        #endregion
    }
}