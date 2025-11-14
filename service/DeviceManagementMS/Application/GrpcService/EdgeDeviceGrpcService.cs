using Grpc.Core;
using Application.Interface.IService;
using HCM.DeviceManagement.Server.gRPC;

namespace Application.GrpcService
{
    public class EdgeDeviceGrpcService : DeviceManagementServer.DeviceManagementServerBase
    {
        private readonly IEdgeDeviceService edgeDeviceService;

        public EdgeDeviceGrpcService(IEdgeDeviceService edgeDeviceService)
        {
            this.edgeDeviceService = edgeDeviceService;
        }

        public override async Task<GetControllerMetaResponse> GetControllerMeta(
            GetControllerMetaRequest request, ServerCallContext context)
        {
            var meta = await edgeDeviceService.GetControllerMetaAsync(request.ControllerKey);
            if (meta == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Controller {request.ControllerKey} not found"));

            var response = new GetControllerMetaResponse
            {
                ControllerKey = meta.ControllerKey,
                BedNumber = meta.BedNumber,
                FirmwareVersion = meta.FirmwareVersion,
                IsActive = meta.IsActive,
                Status = meta.Status,

                EdgeKey = meta.EdgeKey,
                RoomName = meta.RoomName,
                IpAddress = meta.IpAddress,
                Description = meta.Description
            };

            response.Sensors.AddRange(meta.Sensors.Select(s => new SensorMeta
            {
                SensorKey = s.SensorKey,
                Type = s.Type,
                Unit = s.Unit,
                Description = s.Description,
                IsActive = s.IsActive
            }));

            return response;
        }
    }
}
