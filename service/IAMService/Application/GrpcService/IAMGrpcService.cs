using Domain.IRepository;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using HCM.IAM.Server.gRPC;

namespace Application.GrpcService
{
    [AllowAnonymous]
    public class IAMGrpcService : IAMServer.IAMServerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public IAMGrpcService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public override async Task<GetIAMUserResponse> GetIAMUser(
            GetIAMUserRequest request, ServerCallContext context)
        {
            var patient = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberAsync(request.IdentityNumber);

            if (patient == null)
                throw new RpcException(
                    new Status(StatusCode.NotFound, "Patient not found"));

            var response = new GetIAMUserResponse
            {
                FullName = patient.FullName ?? "",
                Address = patient.Address ?? "",
                Gender = patient.Gender ?? "",
                IdentityNumber = patient.IdentityNumber ?? "",
                IsActive = patient.IsActive,
                Email = patient.Email ?? "",
                Phone = patient.Phone ?? "",
                Dob = Timestamp.FromDateTime(DateTime.SpecifyKind(patient.Dob, DateTimeKind.Utc)),
            };

            return response;
        }
    }
}
