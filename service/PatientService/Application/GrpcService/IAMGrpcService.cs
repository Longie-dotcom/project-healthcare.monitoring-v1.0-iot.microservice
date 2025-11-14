using Domain.IRepository;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Application.GrpcService
{
    //[AllowAnonymous]
    public class IAMGrpcService
    {
        private readonly IUnitOfWork unitOfWork;

        public IAMGrpcService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //public override async Task<IAMResponse> GetPatientById(IAMRequest request, ServerCallContext context)
        //{
        //    var patient = await unitOfWork
        //        .GetRepository<IUserRepository>()
        //        .GetByEmailAsync(request.Email);

        //    if (patient == null)
        //        throw new RpcException(new Status(StatusCode.NotFound, "Patient not found"));

        //    return new IAMResponse
        //    {
        //        FullName = patient.FullName,
        //        Address = patient.Address,
        //        Gender = patient.Gender,
        //        IdentityNumber = patient.IdentityNumber,
        //        IsActive = patient.IsActive,
        //        Dob = Timestamp.FromDateTime(patient.Dob.ToUniversalTime()),
        //        Email = patient.Email
        //    };
        //}
    }
}
