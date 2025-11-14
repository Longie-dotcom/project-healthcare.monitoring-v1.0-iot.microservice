using Application.ApplicationException;
using Application.DTO;
using Application.Enum;
using Application.IGrpc;
using Application.Interface.IMessagePublisher;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using HCM.CodeFormatter;

namespace Application.Service
{
    public class PatientService : IPatientService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IIAMGrpcClient iAMGrpcClient;
        private readonly IDeviceManagementGrpcClient deviceManagementGrpcClient;
        private readonly IUpdateUserPublisher updateUserPublisher;
        private readonly ICreateDeviceProfilePublisher createDeviceProfilePublisher;
        #endregion

        #region Properties
        #endregion

        public PatientService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IIAMGrpcClient iAMGrpcClient,
            IDeviceManagementGrpcClient deviceManagementGrpcClient,
            IUpdateUserPublisher updateUserPublisher,
            ICreateDeviceProfilePublisher createDeviceProfilePublisher)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.iAMGrpcClient = iAMGrpcClient;
            this.deviceManagementGrpcClient = deviceManagementGrpcClient;
            this.updateUserPublisher = updateUserPublisher;
            this.createDeviceProfilePublisher = createDeviceProfilePublisher;
        }

        #region Methods
        public async Task<PatientDTO> GetByIdAsync(Guid patientId)
        {
            var repo = unitOfWork.GetRepository<IPatientRepository>();
            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null) 
                throw new PatientNotFound(
                    $"Patient with ID:{patientId} is not found");
            var patientDto = mapper.Map<PatientDTO>(patient);
            return patientDto;
        }

        public async Task<IEnumerable<PatientDTO>> GetAllAsync(
            QueryPatientDTO dto, string? sort)
        {
            var repo = unitOfWork.GetRepository<IPatientRepository>();
            var list = await repo.GetAllWithFilter(
                dto.Search ?? string.Empty, dto.PageIndex, dto.PageLength);

            list = sort?.ToLower() switch
            {
                SortKeyword.SORT_BY_ADMISSIONDATE => list.OrderBy(s => s.AdmissionDate),
                SortKeyword.SORT_BY_DISCHARGEDDATE => list.OrderBy(s => s.DischargeDate),
                _ => list.OrderBy(s => s.PatientCode)
            };

            if (list == null || !list.Any())
                throw new PatientNotFound("The patient list is empty");

            return mapper.Map<IEnumerable<PatientDTO>>(list);
        }

        public async Task<PatientDTO> CreateAsync(PatientCreateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            // Check if identity number exists
            var existing = (await repo.GetPatientByIdentityNumber(dto.IdentityNumber));
            if (existing != null)
            {
                if (existing.IsActive)
                {
                    throw new IdentityNumberExisted(
                        $"The identity number {dto.IdentityNumber} is used");
                }
                else
                {
                    existing.UpdateActive(true);
                    await unitOfWork.CommitAsync(dto.PerformedBy);
                    return mapper.Map<PatientDTO>(existing);
                }
            }

            // Validate patient via IAM
            var user = await iAMGrpcClient.GetUser(dto.IdentityNumber);
            if (user == null)
                throw new UserNotFound(
                    $"User with identity number {dto.IdentityNumber} is not found");

            // Calculate next index
            var todayPatients = (await repo.GetAllAsync())
                .ToList();
            var nextIndex = todayPatients.Count + 1;

            // Generate code using central formatter
            var patientCode = CodeFormatter.Format("PAT", dto.IdentityNumber, DateTime.UtcNow, nextIndex);

            // Create patient
            var patient = new Patient(
                Guid.NewGuid(),
                patientCode,
                PatientStatusEnum.PreAdmitted,
                dto.AdmissionDate ?? DateTime.UtcNow,
                dto.IdentityNumber,
                user.Email,
                user.FullName,
                user.DateOfBirth,
                user.Address,
                user.Gender,
                user.PhoneNumber
            );

            repo.Add(patient);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            return mapper.Map<PatientDTO>(patient);
        }

        public async Task<PatientDTO> UpdateAsync(Guid patientId, PatientUpdateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            // Validate patient existence
            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound(
                    $"Patient with ID:{patientId} is not found");
            var patientDto = mapper.Map<PatientDTO>(patient);

            if (!string.IsNullOrEmpty(dto.PatientStatusCode))
            {
                // Validate status code
                var statusCode = await unitOfWork
                    .GetRepository<IPatientStatusRepository>()
                    .GetPatientStatusByCode(dto.PatientStatusCode);
                if (statusCode == null)
                    throw new PatientStatusCodeNotFound(
                        $"Patient status: {dto.PatientStatusCode}");
                patient.UpdateStatus(dto.PatientStatusCode);
            }

            if (dto.DischargeDate != null)
                patient.SetDischargeDate(dto.DischargeDate ?? DateTime.UtcNow);

            if (dto.AdmissionDate != null)
                patient.UpdateAdmissionDate(dto.AdmissionDate ?? DateTime.UtcNow);

            // Email and phone can not be updated from patient service
            if (!string.IsNullOrEmpty(dto.FullName))
                patient.UpdateFullName(dto.FullName);

            if (!string.IsNullOrEmpty(dto.Address))
                patient.UpdateAddress(dto.Address);

            if (!string.IsNullOrEmpty(dto.Gender))
                patient.UpdateGender(dto.Gender);

            if (dto.DateOfBirth != null)
                patient.UpdateDob(dto.DateOfBirth ?? DateTime.MinValue);

            await unitOfWork.CommitAsync(dto.PerformedBy);

            await updateUserPublisher.PublishAsync(
                new IAMSyncUpdateDTO()
                {
                    PerformedBy = dto.PerformedBy,
                    IdentityNumber = patient.IdentityNumber,
                    Address = dto.Address,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth,
                    FullName = dto.FullName,
                });

            return mapper.Map<PatientDTO>(patient);
        }

        public async Task SyncUpdateAsync(IAMSyncUpdateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var repo = unitOfWork.GetRepository<IPatientRepository>();

            // Validate patient existence
            var patient = await repo.GetPatientByIdentityNumber(dto.IdentityNumber);
            if (patient == null)
                throw new PatientNotFound(
                    $"Patient with identity number:{dto.IdentityNumber} is not found");
            var patientDto = mapper.Map<PatientDTO>(patient);

            if (!string.IsNullOrEmpty(dto.Email))
                patient.UpdateEmail(dto.Email);

            if (!string.IsNullOrEmpty(dto.FullName))
                patient.UpdateFullName(dto.FullName);

            if (!string.IsNullOrEmpty(dto.Address))
                patient.UpdateAddress(dto.Address);

            if (!string.IsNullOrEmpty(dto.Gender))
                patient.UpdateGender(dto.Gender);

            if (dto.DateOfBirth != null)
                patient.UpdateDob(dto.DateOfBirth ?? DateTime.MinValue);

            if (!string.IsNullOrEmpty(dto.Phone))
                patient.UpdatePhone(dto.Phone);

            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task DeleteAsync(Guid patientId, PatientDeleteDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IPatientRepository>();
            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound(
                    $"Patient with ID:{patientId} is not found");
            patient.UpdateActive(false);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task SyncDeleteAsync(IAMSyncDeleteDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IPatientRepository>();
            var patient = await repo.GetPatientByIdentityNumber(dto.IdentityNumber);
            if (patient == null)
                throw new PatientNotFound(
                    $"Patient with identity number:{dto.IdentityNumber} is not found");
            patient.UpdateActive(false);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task AssignBedAsync(Guid patientId, AssignBedDTO dto)
        {
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound($"Patient with ID:{patientId} not found.");

            // Validate and get controller details from DeviceManagementMS
            var controllerProfile = await deviceManagementGrpcClient
                .GetControllerMetaAsync(dto.ControllerKey);
            if (controllerProfile == null)
                throw new DeviceNotFound(
                    $"Controller with key {dto.ControllerKey} not found in DeviceManagementMS.");

            // Begin DB transaction
            await unitOfWork.BeginTransactionAsync();

            var bedAssignment = new PatientBedAssignment(
                Guid.NewGuid(),
                patientId,
                dto.ControllerKey,
                DateTime.UtcNow
            );

            // Apply domain
            patient.AssignBed(bedAssignment);
            patient.UpdateStatus(PatientStatusEnum.Admitted);

            // Apply persistence
            repo.AddBedAssignment(bedAssignment);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Map to CreateDeviceProfileDTO for DataCollectionMS
            var createProfileDto = new CreateDeviceProfileDTO
            {
                PatientIdentityNumber = patient.IdentityNumber,
                FullName = patient.FullName,
                Dob = patient.Dob,
                Gender = patient.Gender,
                Email = patient.Email,
                Phone = patient.Phone,
                BedAssignedAt = bedAssignment.AssignedAt,
                ControllerKey = controllerProfile.ControllerKey,
                BedNumber = controllerProfile.BedNumber,
                EdgeKey = controllerProfile.EdgeKey,
                RoomName = controllerProfile.RoomName,
                IpAddress = controllerProfile.IpAddress,
                FirmwareVersion = controllerProfile.FirmwareVersion,
                IsActive = true,

                // Combine controller’s sensors (from DeviceManagementMS)
                PatientSensors = controllerProfile.Sensors.Select(s => new PatientSensorDTO
                {
                    SensorKey = s.SensorKey,
                    AssignedAt = DateTime.UtcNow,
                    UnassignedAt = null,
                    IsActive = true
                }).ToList(),

                // Include patient’s staff assignment if exists
                PatientStaffs = patient.PatientStaffAssignment.Select(st => new PatientStaffDTO
                {
                    StaffIdentityNumber = st.StaffIdentityNumber,
                    AssignedAt = st.AssignedAt,
                    UnassignedAt = st.UnassignedAt,
                    IsActive = st.IsActive
                }).ToList()
            };

            // Publish event to DataCollectionMS
            await createDeviceProfilePublisher.PublishDeviceProfileAsync(createProfileDto);
        }

        public async Task ReleaseBedAsync(Guid patientId, ReleaseBedDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound($"Patient with ID:{patientId} not found.");

            // Domain logic to release
            patient.ReleaseBed(dto.PatientBedAssignmentID, DateTime.UtcNow);
            patient.UpdateStatus(PatientStatusEnum.Discharged);

            // Get the released assignment
            var releasedBed = patient.PatientBedAssignments
                .FirstOrDefault(b => b.PatientBedAssignmentID == dto.PatientBedAssignmentID);

            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task AssignStaffAsync(Guid patientId, AssignStaffDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound($"Patient with ID:{patientId} not found.");

            // Validate patient via IAM
            var user = await iAMGrpcClient.GetUser(dto.StaffIdentityNumber);
            if (user == null)
                throw new UserNotFound(
                    $"Staff with identity number {dto.StaffIdentityNumber} is not found");


            var staffAssignment = new PatientStaffAssignment(
                Guid.NewGuid(),
                patientId,
                dto.StaffIdentityNumber,
                DateTime.UtcNow,
                true
            );

            // Domain logic
            patient.AssignStaff(staffAssignment);

            // Explicitly tell EF to insert
            repo.AddStaffAssignment(staffAssignment);

            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task UnassignStaffAsync(Guid patientId, UnassignStaffDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound($"Patient with ID:{patientId} not found.");

            // Domain logic
            patient.UnassignStaff(dto.PatientStaffAssignmentID, DateTime.UtcNow);

            // Get the unassigned record to remove
            var unassigned = patient.PatientStaffAssignment
                .FirstOrDefault(s => s.PatientStaffAssignmentID == dto.PatientStaffAssignmentID);

            await unitOfWork.CommitAsync(dto.PerformedBy);
        }
        #endregion
    }
}
