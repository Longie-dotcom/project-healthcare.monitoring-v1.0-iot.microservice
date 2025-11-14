using Grpc.Core;
using HCM.Patient.Server.gRPC;
using Domain.IRepository;

namespace Application.GrpcService
{
    public class PatientGrpcService : PatientServer.PatientServerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public PatientGrpcService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public override async Task<IsControllerInUseResponse> IsControllerInUse(
            IsControllerInUseRequest request,
            ServerCallContext context)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ControllerKey))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Controller key is required"));

            try
            {
                var repo = unitOfWork.GetRepository<IPatientRepository>();
                var inUse = await repo.IsControllerInUseAsync(request.ControllerKey);

                return new IsControllerInUseResponse
                {
                    IsInUse = inUse
                };
            }
            catch (RpcException) { throw; } // rethrow intentional RPC errors
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
