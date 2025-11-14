using Application.ApplicationException;
using Application.Interface.IGrpc;
using Grpc.Core;
using Grpc.Net.Client;
using HCM.Patient.Server.gRPC;
using Infrastructure.InfrastructureException;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Grpc
{
    public class PatientGrpcClient : IPatientGrpcClient
    {
        #region Attributes
        private readonly PatientServer.PatientServerClient client;
        #endregion

        #region Constructor
        public PatientGrpcClient(IConfiguration configuration)
        {
            var grpcServerUrl = configuration["GRPC_PATIENT"];
            if (string.IsNullOrEmpty(grpcServerUrl))
                throw new GrpcCommunicationException("gRPC Patient server URL is missing.");

            var channel = GrpcChannel.ForAddress(grpcServerUrl);
            client = new PatientServer.PatientServerClient(channel);
        }
        #endregion

        #region Methods
        public async Task<bool> IsControllerInUseAsync(string controllerKey)
        {
            try
            {
                var response = await client.IsControllerInUseAsync(
                    new IsControllerInUseRequest
                    {
                        ControllerKey = controllerKey
                    });

                return response.IsInUse;
            }
            catch (RpcException ex)
            {
                // Keep consistent with your IAMGrpcClient pattern
                throw new Exception(
                    $"Failed to check controller usage for '{controllerKey}'. gRPC error: {ex.Status.Detail}");
            }
        }
        #endregion
    }
}
