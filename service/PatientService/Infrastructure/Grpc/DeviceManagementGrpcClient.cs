using Application.ApplicationException;
using Application.DTO;
using Application.IGrpc;
using Grpc.Core;
using Grpc.Net.Client;
using HCM.DeviceManagement.Server.gRPC;
using Infrastructure.InfrastructureException;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Grpc
{
    public class DeviceManagementGrpcClient : IDeviceManagementGrpcClient
    {
        #region Attributes
        private readonly DeviceManagementServer.DeviceManagementServerClient client;
        #endregion

        #region Properties
        #endregion

        public DeviceManagementGrpcClient(IConfiguration configuration)
        {
            var grpcServerUrl = configuration["GRPC_DEVICE_MANAGEMENT"];
            if (string.IsNullOrEmpty(grpcServerUrl))
                throw new GrpcCommunicationException("DeviceManagement gRPC server URL is missing.");

            var channel = GrpcChannel.ForAddress(grpcServerUrl);
            client = new DeviceManagementServer.DeviceManagementServerClient(channel);
        }

        /// <summary>
        /// Get controller meta information including attached sensors and edge device info
        /// </summary>
        public async Task<DeviceProfileControllerDTO> GetControllerMetaAsync(string controllerKey)
        {
            try
            {
                var response = await client.GetControllerMetaAsync(new GetControllerMetaRequest
                {
                    ControllerKey = controllerKey
                });

                return new DeviceProfileControllerDTO
                {
                    // Controller info
                    ControllerKey = response.ControllerKey,
                    BedNumber = response.BedNumber,
                    FirmwareVersion = response.FirmwareVersion,
                    IsActive = response.IsActive,
                    Status = response.Status,

                    // Edge device info
                    EdgeKey = response.EdgeKey,
                    RoomName = response.RoomName,
                    IpAddress = response.IpAddress,
                    Description = response.Description,

                    // Sensors
                    Sensors = response.Sensors.Select(s => new DeviceProfileSensorDTO
                    {
                        SensorKey = s.SensorKey,
                        Type = s.Type,
                        Unit = s.Unit,
                        Description = s.Description,
                        IsActive = s.IsActive
                    }).ToList()
                };
            }
            catch (RpcException ex)
            {
                throw new DeviceNotFound(
                    $"Controller with key: '{controllerKey}' was not found in DeviceManagementMS.");
            }
        }
    }
}
