using Application.ApplicationException;
using Application.DTO;
using Application.IGrpc;
using Grpc.Core;
using Grpc.Net.Client;
using HCM.IAM.Server.gRPC;
using Infrastructure.InfrastructureException;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Grpc
{
    public class IAMGrpcClient : IIAMGrpcClient
    {
        #region Attributes
        private readonly IAMServer.IAMServerClient client;
        #endregion

        #region Properties
        #endregion

        public IAMGrpcClient(IConfiguration configuration)
        {
            var grpcServerUrl = configuration["GRPC_IAM"];
            if (string.IsNullOrEmpty(grpcServerUrl))
                throw new GrpcCommunicationException("gRPC server URL is missing.");

            var channel = GrpcChannel.ForAddress(grpcServerUrl);
            client = new IAMServer.IAMServerClient(channel);
        }

        public async Task<UserDTO> GetUser(string identityNumber)
        {
            try
            {
                var response = await client.GetIAMUserAsync(new GetIAMUserRequest
                {
                    IdentityNumber = identityNumber
                });

                return new UserDTO
                {
                    FullName = response.FullName,
                    Address = response.Address,
                    Gender = response.Gender,
                    IdentityNumber = response.IdentityNumber,
                    Email = response.Email,
                    PhoneNumber = response.Phone,
                    DateOfBirth = response.Dob.ToDateTime(),
                };
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
            {
                throw new UserNotFound(
                    $"User with identity number: '{identityNumber}' was not found in IAMMS.");
            }
        }
    }
}
