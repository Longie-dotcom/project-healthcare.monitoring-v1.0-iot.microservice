using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IMessagePublisher;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using HCM.CodeFormatter;

namespace Application.Service
{
    public class EdgeDeviceService : IEdgeDeviceService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUpdateDevicePublisher updateDevicePublisher;

        public EdgeDeviceService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IUpdateDevicePublisher updateDevicePublisher)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.updateDevicePublisher = updateDevicePublisher;
        }

        #region Methods
        public async Task<EdgeDeviceDTO> GetByIdAsync(
            Guid edgeDeviceId)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();
            var device = await repo.GetDetailedByIdAsync(edgeDeviceId);
            if (device == null) 
                throw new DeviceNotFound($"EdgeDevice {edgeDeviceId} not found");
            return mapper.Map<EdgeDeviceDTO>(device);
        }

        public async Task<IEnumerable<EdgeDeviceDTO>> GetAllAsync(
            QueryEdgeDeviceDTO dto, string sort)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();
            var devices = await repo.GetAllWithFilter(
                dto.Search ?? "", 
                dto.PageIndex, 
                dto.PageLength);

            if (devices == null || !devices.Any())
                throw new DeviceNotFound("The device list is empty");

            return mapper.Map<IEnumerable<EdgeDeviceDTO>>(devices);
        }

        public async Task<EdgeDeviceDTO> CreateAsync(EdgeDeviceCreateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // Validate room occupied
            var existed = await repo.IsRoomAssignedAsync(dto.RoomName);
            if (existed)
                throw new RoomOccupied($"The room: {dto.RoomName} has been occupied");

            // Fetch all existing devices (you could optimize this to just count)
            var existingDevices = (await repo.GetAllAsync()).ToList();
            var nextIndex = existingDevices.Count + 1;

            // Generate random 8-character alphanumeric anchor
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var anchor = new string(Enumerable.Range(0, 8)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());

            // Generate EdgeKey using the random anchor
            var edgeKey = CodeFormatter.Format("EDGE", anchor, DateTime.UtcNow, nextIndex);

            var device = new EdgeDevice(
                Guid.NewGuid(),
                edgeKey,
                dto.RoomName,
                dto.IpAddress,
                dto.Description ?? string.Empty
            );

            repo.Add(device);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            return mapper.Map<EdgeDeviceDTO>(device);
        }

        public async Task<EdgeDeviceDTO> UpdateAsync(
            Guid edgeDeviceId, EdgeDeviceUpdateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();
            var device = await repo.GetDetailedByIdAsync(edgeDeviceId);
            if (device == null)
                throw new DeviceNotFound($"EdgeDevice {edgeDeviceId} not found");

            // Update main EdgeDevice fields
            if (!string.IsNullOrWhiteSpace(dto.RoomName) && dto.RoomName != device.RoomName)
            {
                // Validate room occupied
                var existed = await repo.IsRoomAssignedAsync(dto.RoomName);
                if (existed)
                    throw new RoomOccupied($"The room: {dto.RoomName} has been occupied");

                device.UpdateRoomName(dto.RoomName);
            }

            if (!string.IsNullOrWhiteSpace(dto.IpAddress))
                device.UpdateIpAddress(dto.IpAddress);

            if (!string.IsNullOrWhiteSpace(dto.Description))
                device.UpdateDescription(dto.Description);

            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Publish update event
            await PublishEdgeDeviceAsync(device, dto.PerformedBy);

            // Return mapped DTO for API response
            return mapper.Map<EdgeDeviceDTO>(device);
        }

        public async Task DeleteAsync(
            Guid edgeDeviceId, EdgeDeviceDeleteDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // Apply domain
            var device = await repo.GetDetailedByIdAsync(edgeDeviceId);
            if (device == null)
                throw new DeviceNotFound($"EdgeDevice: {edgeDeviceId} not found");
            device.Deactivate();
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Publish deletion event
            await PublishEdgeDeviceAsync(device, dto.PerformedBy);
        }

        public async Task AssignControllerAsync(ControllerCreateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // Fetch the EdgeDevice owner
            var edgeOwner = await repo.GetEdgeDeviceByEdgeKeyAsTracking(dto.EdgeKey);
            if (edgeOwner == null)
                throw new DeviceNotFound(
                    $"Edge device with key:{dto.EdgeKey} is not found");

            // Validate bed occupied
            if (string.IsNullOrEmpty(dto.BedNumber))
            {
                var existed = await repo.IsBedAssignedAsync(dto.EdgeKey, dto.BedNumber ?? "");
                if (existed)
                    throw new RoomOccupied(
                        $"The bed: {dto.BedNumber} has been occupied in the room own by edge: {dto.EdgeKey}");
            }

            // Generate ControllerKey using parent EdgeKey as the second parameter
            var existingControllers = edgeOwner.Controllers.ToList();
            var nextIndex = existingControllers.Count + 1;
            var controllerKey = CodeFormatter.Format("CTRL", edgeOwner.EdgeKey, DateTime.UtcNow, nextIndex);

            var controller = new Controller(
                Guid.NewGuid(),
                controllerKey,
                dto.EdgeKey,
                dto.BedNumber ?? "",
                dto.IpAddress,
                dto.FirmwareVersion ?? ""
            );

            // Apply domain
            edgeOwner.AddController(controller);

            // Apply persistence
            repo.AssignController(controller);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task AssignSensorAsync(SensorCreateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // Fetch Controller owner
            var controllerOwner = await repo.GetControllerByControllerKeyAsTracking(dto.ControllerKey);
            if (controllerOwner == null)
                throw new DeviceNotFound(
                    $"Controller device with key:{dto.ControllerKey} is not found");

            // Generate SensorKey using parent ControllerKey as the second parameter
            var existingSensors = controllerOwner.Sensors.ToList();
            var nextIndex = existingSensors.Count + 1;
            var sensorKey = CodeFormatter.Format("SENS", controllerOwner.ControllerKey, DateTime.UtcNow, nextIndex);

            var sensor = new Sensor(
                Guid.NewGuid(),
                sensorKey,
                dto.ControllerKey,
                dto.Type,
                dto.Unit,
                dto.Description ?? ""
            );

            // Apply domain
            controllerOwner.AddSensor(sensor);

            // Apply persistence
            repo.AssignSensor(sensor);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Publish sensor sync
            var syncDto = new SensorSyncDTO
            {
                SensorKey = sensor.SensorKey,
                ControllerKey = sensor.ControllerKey,
                Type = sensor.Type,
                Unit = sensor.Unit,
                Description = sensor.Description,
                IsActive = sensor.IsActive,
                PerformedBy = dto.PerformedBy
            };

            await updateDevicePublisher.PublishUpdateSensorAsync(syncDto);
        }

        public async Task UnassignControllerAsync(ControllerDeleteDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // Apply domain
            var controller = await repo.GetControllerByControllerKeyAsTracking(dto.ControllerKey);
            if (controller == null)
                throw new DeviceNotFound(
                    $"Controller device with key:{dto.ControllerKey} is not found");
            controller.Deactivate();
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Publish sensor sync
            foreach (var sensor in controller.Sensors)
            {
                var sensorSyncDto = new SensorSyncDTO
                {
                    SensorKey = sensor.SensorKey,
                    IsActive = false,
                    PerformedBy = dto.PerformedBy
                };

                await updateDevicePublisher.PublishUpdateSensorAsync(sensorSyncDto);
            }

            // Publish controller sync
            var syncDto = new ControllerSyncDTO
            {
                ControllerKey = controller.ControllerKey,
                IsActive = false,
                PerformedBy = dto.PerformedBy
            };
            await updateDevicePublisher.PublishUpdateControllerAsync(syncDto);
        }

        public async Task UnassignSensorAsync(SensorDeleteDTO dto)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            // Apply domain
            var sensor = await repo.GetSensorBySensorKeyAsTracking(dto.SensorKey);
            if (sensor == null)
                throw new DeviceNotFound(
                    $"Sensor device with key:{dto.SensorKey} is not found");
            sensor.UpdateActive(false);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Publish sensor sync
            var syncDto = new SensorSyncDTO
            {
                ControllerKey = sensor.ControllerKey,
                SensorKey = dto.SensorKey,
                IsActive = false,
                PerformedBy = dto.PerformedBy
            };

            await updateDevicePublisher.PublishUpdateSensorAsync(syncDto);
        }

        public async Task ReactivateEdgeDeviceAsync(string edgeKey, string performedBy)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            var device = await repo.GetEdgeDeviceByEdgeKeyAsTrackingInactive(edgeKey);
            if (device == null)
                throw new DeviceNotFound(
                    $"EdgeDevice with key '{edgeKey}' not found or already active.");

            // Force activation
            device.Activate();
            await unitOfWork.CommitAsync(performedBy);

            // Publish all controllers and sensors
            foreach (var controller in device.Controllers)
            {
                var controllerSync = new ControllerSyncDTO
                {
                    ControllerKey = controller.ControllerKey,
                    BedNumber = controller.BedNumber,
                    IpAddress = controller.IpAddress,
                    FirmwareVersion = controller.FirmwareVersion,
                    IsActive = controller.IsActive,
                    PerformedBy = performedBy
                };
                await updateDevicePublisher.PublishUpdateControllerAsync(controllerSync);

                foreach (var sensor in controller.Sensors)
                {
                    var sensorSync = new SensorSyncDTO
                    {
                        SensorKey = sensor.SensorKey,
                        ControllerKey = controller.ControllerKey,
                        Type = sensor.Type,
                        Unit = sensor.Unit,
                        Description = sensor.Description,
                        IsActive = sensor.IsActive,
                        PerformedBy = performedBy
                    };
                    await updateDevicePublisher.PublishUpdateSensorAsync(sensorSync);
                }
            }
        }

        public async Task ReactivateControllerAsync(string controllerKey, string performedBy)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();
            var controller = await repo.GetControllerByControllerKeyAsTrackingInactive(controllerKey);
            if (controller == null)
                throw new DeviceNotFound(
                    $"Controller with key '{controllerKey}' not found or already active.");

            // Force activation
            controller.Activate();
            await unitOfWork.CommitAsync(performedBy);

            var controllerSync = new ControllerSyncDTO
            {
                ControllerKey = controller.ControllerKey,
                BedNumber = controller.BedNumber,
                IpAddress = controller.IpAddress,
                FirmwareVersion = controller.FirmwareVersion,
                IsActive = controller.IsActive,
                PerformedBy = performedBy
            };
            await updateDevicePublisher.PublishUpdateControllerAsync(controllerSync);

            foreach (var sensor in controller.Sensors)
            {
                var sensorSync = new SensorSyncDTO
                {
                    SensorKey = sensor.SensorKey,
                    ControllerKey = controller.ControllerKey,
                    Type = sensor.Type,
                    Unit = sensor.Unit,
                    Description = sensor.Description,
                    IsActive = sensor.IsActive,
                    PerformedBy = performedBy
                };
                await updateDevicePublisher.PublishUpdateSensorAsync(sensorSync);
            }
        }

        public async Task ReactivateSensorAsync(string sensorKey, string performedBy)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();
            var sensor = await repo.GetSensorBySensorKeyAsTrackingInactive(sensorKey);
            if (sensor == null)
                throw new DeviceNotFound(
                    $"Sensor with key '{sensorKey}' not found or already active.");

            // Force activation
            sensor.UpdateActive(true);
            await unitOfWork.CommitAsync(performedBy);

            var sensorSync = new SensorSyncDTO
            {
                SensorKey = sensor.SensorKey,
                ControllerKey = sensor.ControllerKey,
                Type = sensor.Type,
                Unit = sensor.Unit,
                Description = sensor.Description,
                IsActive = sensor.IsActive,
                PerformedBy = performedBy
            };
            await updateDevicePublisher.PublishUpdateSensorAsync(sensorSync);
        }

        // Grpc service for Patient MS
        public async Task<DeviceProfileControllerDTO> GetControllerMetaAsync(string controllerKey)
        {
            var repo = unitOfWork.GetRepository<IEdgeDeviceRepository>();

            var controller = await repo.GetControllerByControllerKeyAsTracking(controllerKey);
            if (controller == null)
                throw new DeviceNotFound($"{controllerKey} is not a not found");

            var edge = await repo.GetEdgeDeviceByEdgeKey(controller.EdgeKey);
            if (edge == null)
                throw new DeviceNotFound($"{controller.EdgeKey} is not a not found");


            return new DeviceProfileControllerDTO
            {
                // Controller info
                ControllerKey = controller.ControllerKey,
                BedNumber = controller.BedNumber,
                FirmwareVersion = controller.FirmwareVersion,
                IsActive = controller.IsActive,
                Status = controller.Status,

                // Edge device info
                EdgeKey = edge?.EdgeKey ?? string.Empty,
                RoomName = edge?.RoomName ?? string.Empty,
                IpAddress = edge?.IpAddress ?? string.Empty,
                Description = edge?.Description ?? string.Empty,

                // Sensors
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
        #endregion

        #region Private Helpers
        private async Task PublishEdgeDeviceAsync(EdgeDevice device, string performedBy)
        {
            // Publish EdgeDevice Sync
            foreach (var controller in device.Controllers)
            {
                var syncDto = new EdgeDeviceSyncDTO
                {
                    IpAddress = null,
                    IsActive = controller.IsActive,
                    ControllerKey = controller.ControllerKey,
                    RoomName = device.RoomName,
                    PerformedBy = performedBy
                };
                await updateDevicePublisher.PublishUpdateEdgeAsync(syncDto);

                await PublishControllerAsync(controller, performedBy);
            }
        }

        private async Task PublishControllerAsync(Controller controller, string performedBy)
        {
            // Publish the controller first
            var controllerDto = new ControllerSyncDTO
            {
                ControllerKey = controller.ControllerKey,
                BedNumber = controller.BedNumber,
                FirmwareVersion = controller.FirmwareVersion,
                IpAddress = controller.IpAddress,
                IsActive = controller.IsActive,
                PerformedBy = performedBy
            };
            await updateDevicePublisher.PublishUpdateControllerAsync(controllerDto);

            // Publish all child sensors
            foreach (var sensor in controller.Sensors)
            {
                await PublishSensorAsync(sensor, performedBy);
            }
        }

        private async Task PublishSensorAsync(Sensor sensor, string performedBy)
        {
            var sensorDto = new SensorSyncDTO
            {
                SensorKey = sensor.SensorKey,
                ControllerKey = sensor.ControllerKey,
                Type = sensor.Type,
                Unit = sensor.Unit,
                Description = sensor.Description,
                IsActive = sensor.IsActive,
                PerformedBy = performedBy
            };
            await updateDevicePublisher.PublishUpdateSensorAsync(sensorDto);
        }
        #endregion
    }
}