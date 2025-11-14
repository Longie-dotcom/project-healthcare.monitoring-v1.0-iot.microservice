using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.IRepository;

namespace Application.Service
{
    public class PatientStatusService : IPatientStatusService
    {
        #region Properties
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        #endregion

        #region Properties
        #endregion

        public PatientStatusService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        #region Methods
        public async Task<IEnumerable<PatientStatusDTO>> GetAllAsync()
        {
            var statuses = await unitOfWork
                .GetRepository<IPatientStatusRepository>()
                .GetAllAsync();
            if (statuses == null || !statuses.Any())
                throw new PatientStatusCodeNotFound(
                    $"Patient status list is not found");

            return mapper.Map<IEnumerable<PatientStatusDTO>>(statuses);
        }

        public async Task<PatientStatusDTO> GetByCodeAsync(
            string statusCode)
        {
            var status = await unitOfWork
                .GetRepository<IPatientStatusRepository>()
                .GetPatientStatusByCode(statusCode);
            if (status == null)
                throw new PatientStatusCodeNotFound($"Patient status: {statusCode}");

            return mapper.Map<PatientStatusDTO>(status);
        }

        public async Task<PatientStatusDTO> CreateAsync(
            PatientStatusCreateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var patientStatus = new PatientStatus(
                dto.PatientStatusCode, dto.Description, dto.Name);

            // Validate duplication
            var existed = await unitOfWork
                .GetRepository<IPatientStatusRepository>()
                .GetPatientStatusByCode(dto.PatientStatusCode);
            if (existed != null)
                throw new PatientStatusCodeExisted(
                    $"The status code: {dto.PatientStatusCode} is existed");

            unitOfWork
                .GetRepository<IPatientStatusRepository>()
                .Add(patientStatus);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            return mapper.Map<PatientStatusDTO>(patientStatus);
        }

        public async Task DeleteAsync(
            string statusCode, PatientStatusDeleteDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var status = await unitOfWork
                .GetRepository<IPatientStatusRepository>()
                .GetPatientStatusByCode(statusCode);
            if (status == null)
                throw new PatientStatusCodeNotFound($"Patient status: {statusCode}");
            
            await unitOfWork
                .GetRepository<IPatientStatusRepository>()
                .DeletePatientStatusByCode(statusCode);

            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task<PatientStatusDTO> UpdateAsync(
            string statusCode, PatientStatusUpdateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var status = await unitOfWork
                .GetRepository<IPatientStatusRepository>()
                .GetPatientStatusByCode(statusCode);
            if (status == null)
                throw new PatientStatusCodeNotFound($"Patient status: {statusCode}");

            if (!string.IsNullOrEmpty(dto.Description))
                status.UpdateDescription(dto.Description);
            if (!string.IsNullOrEmpty(dto.Name))
                status.UpdateName(dto.Name);

            await unitOfWork.CommitAsync(dto.PerformedBy);

            return mapper.Map<PatientStatusDTO>(status);
        }
        #endregion
    }
}
