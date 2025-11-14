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
using HCM.MessageBrokerDTOs;

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
        private readonly IDeviceProfileAssignmentPublisher deviceProfileAssignmentPublisher;
        private readonly IStaffAssignmentPublisher staffAssignmentPublisher;
        #endregion

        #region Properties
        #endregion

        public PatientService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IIAMGrpcClient iAMGrpcClient,
            IDeviceManagementGrpcClient deviceManagementGrpcClient,
            IUpdateUserPublisher updateUserPublisher,
            IDeviceProfileAssignmentPublisher deviceProfileAssignmentPublisher,
            IStaffAssignmentPublisher staffAssignmentPublisher)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.iAMGrpcClient = iAMGrpcClient;
            this.deviceManagementGrpcClient = deviceManagementGrpcClient;
            this.updateUserPublisher = updateUserPublisher;
            this.deviceProfileAssignmentPublisher = deviceProfileAssignmentPublisher;
            this.staffAssignmentPublisher = staffAssignmentPublisher;
        }

        #region Methods
        public async Task<PatientDTO> GetByIdAsync(Guid patientId)
        {
            var repo = unitOfWork.GetRepository<IPatientRepository>();
            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound($"Patient with ID:{patientId} not found or has been deactivated.");

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
                throw new PatientNotFound($"Patient with ID:{patientId} not found or has been deactivated.");

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
                new UpdateUser()
                {
                    PerformedBy = dto.PerformedBy ?? "",
                    IdentityNumber = patient.IdentityNumber,
                    Address = patient.Address ?? "",
                    Gender = patient.Gender ?? "",
                    Dob = patient.Dob,
                    Name = patient.FullName ?? "",
                });

            return mapper.Map<PatientDTO>(patient);
        }

        public async Task SyncUpdateAsync(UpdateUser dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var repo = unitOfWork.GetRepository<IPatientRepository>();

            // Validate patient existence
            var patient = await repo.GetPatientByIdentityNumber(dto.IdentityNumber);
            if (patient == null)
                throw new PatientNotFound(
                    $"Patient with identity number:{dto.IdentityNumber} is not found.");
            var patientDto = mapper.Map<PatientDTO>(patient);

            if (!string.IsNullOrEmpty(dto.Email))
                patient.UpdateEmail(dto.Email);

            if (!string.IsNullOrEmpty(dto.Name))
                patient.UpdateFullName(dto.Name);

            if (!string.IsNullOrEmpty(dto.Address))
                patient.UpdateAddress(dto.Address);

            if (!string.IsNullOrEmpty(dto.Gender))
                patient.UpdateGender(dto.Gender);

            if (dto.Dob != null)
                patient.UpdateDob(dto.Dob ?? DateTime.MinValue);

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
                throw new PatientNotFound($"Patient with ID:{patientId} not found or has been deactivated.");
            patient.UpdateActive(false);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task SyncDeleteAsync(DeleteUser dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IPatientRepository>();
            var patient = await repo.GetPatientByIdentityNumber(dto.IdentityNumber);
            if (patient == null)
                throw new PatientNotFound(
                    $"Patient with identity number:{dto.IdentityNumber} is not found.");
            patient.UpdateActive(false);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task<string> AssignBedAsync(Guid patientId, AssignBedDTO dto)
        {
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            // Validate patient existence
            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound($"Patient with ID:{patientId} not found or has been deactivated.");

            // Validate and populate device information
            var controllerProfile = await deviceManagementGrpcClient
                .GetControllerMetaAsync(dto.ControllerKey);
            if (controllerProfile == null)
                throw new DeviceNotFound($"Controller with key {dto.ControllerKey} not found.");

            await unitOfWork.BeginTransactionAsync();

            // Apply domain
            var newBed = new PatientBedAssignment(
                Guid.NewGuid(),
                patientId,
                dto.ControllerKey,
                DateTime.UtcNow
            );
            string message = "Assign patient to bed successfully";
            var previousBed = patient.AssignBed(newBed);
            if (previousBed != null)
                message = "Move patient to other bed successfully";
            patient.UpdateStatus(PatientStatusEnum.Admitted);

            // Apply persistence
            repo.AddBedAssignment(newBed);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Publish previous unassign (if existed)
            if (previousBed != null)
                await PublishBedUnassignAsync(patient, previousBed, dto.PerformedBy);

            // Publish new assignment
            await PublishBedAssignAsync(patient, newBed, dto.PerformedBy);

            return message;
        }

        public async Task ReleaseBedAsync(Guid patientId, ReleaseBedDTO dto)
        {
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            // Validate patient existence
            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound($"Patient with ID:{patientId} not found.");
            
            await unitOfWork.BeginTransactionAsync();

            // Apply domain
            var releasedBed = patient.ReleaseBed(dto.PatientBedAssignmentID, DateTime.UtcNow);
            patient.UpdateStatus(PatientStatusEnum.Discharged);

            // Apply persistence
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Publish unassignment
            await PublishBedUnassignAsync(patient, releasedBed, dto.PerformedBy);
        }

        public async Task AssignStaffAsync(Guid patientId, AssignStaffDTO dto)
        {
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            // Validate patient existence
            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound($"Patient with ID:{patientId} not found.");

            // Validate staff existence
            var user = await iAMGrpcClient.GetUser(dto.StaffIdentityNumber);
            if (user == null)
                throw new UserNotFound($"Staff {dto.StaffIdentityNumber} not found.");

            await unitOfWork.BeginTransactionAsync();

            // Apply domain
            var staffAssignment = new PatientStaffAssignment(
                Guid.NewGuid(),
                patientId,
                dto.StaffIdentityNumber,
                DateTime.UtcNow
            );
            patient.AssignStaff(staffAssignment);

            // Apply persistence
            repo.AddStaffAssignment(staffAssignment);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Publish new assignment
            await PublishStaffAssignAsync(patient, staffAssignment, dto.PerformedBy);
        }

        public async Task UnassignStaffAsync(Guid patientId, UnassignStaffDTO dto)
        {
            var repo = unitOfWork.GetRepository<IPatientRepository>();

            // Validate patient existence
            var patient = await repo.GetDetailedByIdAsync(patientId);
            if (patient == null)
                throw new PatientNotFound($"Patient with ID:{patientId} not found.");

            await unitOfWork.BeginTransactionAsync();

            // Apply domain
            var staffAssignment = patient.UnassignStaff(dto.PatientStaffAssignmentID, DateTime.UtcNow);

            // Apply persistence
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Publish unassignment
            await PublishStaffUnassignAsync(patient, staffAssignment, dto.PerformedBy);
        }
        #endregion

        #region Private Helpers
        private async Task PublishBedUnassignAsync(
            Patient patient,
            PatientBedAssignment oldBed,
            string performedBy)
        {
            var controllerProfile = await deviceManagementGrpcClient
                .GetControllerMetaAsync(oldBed.ControllerKey);

            if (controllerProfile == null)
                throw new DeviceNotFound($"Controller {oldBed.ControllerKey} not found.");

            var dto = new DeviceProfileRemove
            {
                IdentityNumber = patient.IdentityNumber,
                ControllerKey = oldBed.ControllerKey,
                EdgeKey = controllerProfile.EdgeKey,
                UnassignedAt = oldBed.ReleasedAt ?? DateTime.UtcNow,
                PerformedBy = performedBy
            };

            await deviceProfileAssignmentPublisher.PublishUnassignDeviceProfile(dto);
        }

        private async Task PublishBedAssignAsync(
            Patient patient,
            PatientBedAssignment newBed,
            string performedBy)
        {
            var controllerProfile = await deviceManagementGrpcClient
                .GetControllerMetaAsync(newBed.ControllerKey);

            if (controllerProfile == null)
                throw new DeviceNotFound($"Controller {newBed.ControllerKey} not found.");

            var dto = new DeviceProfileCreate
            {
                IdentityNumber = patient.IdentityNumber,
                FullName = patient.FullName,
                Dob = patient.Dob,
                Gender = patient.Gender,
                Email = patient.Email,
                Phone = patient.Phone,
                AssignedAt = newBed.AssignedAt,
                ControllerKey = controllerProfile.ControllerKey,
                BedNumber = controllerProfile.BedNumber,
                EdgeKey = controllerProfile.EdgeKey,
                IpAddress = controllerProfile.IpAddress,
                IsActive = true,
                PerformedBy = performedBy,

                PatientSensors = controllerProfile.Sensors.Select(s => new PatientSensorDTO
                {
                    SensorKey = s.SensorKey,
                    AssignedAt = DateTime.UtcNow
                }).ToList(),

                PatientStaffs = patient.PatientStaffAssignment.Select(st => new PatientStaffDTO
                {
                    StaffIdentityNumber = st.StaffIdentityNumber,
                    AssignedAt = st.AssignedAt
                }).ToList()
            };

            await deviceProfileAssignmentPublisher.PublishAssignDeviceProfile(dto);
        }

        private async Task PublishStaffAssignAsync(
            Patient patient,
            PatientStaffAssignment staffAssignment,
            string performedBy)
        {
            var dto = new PatientStaffCreate
            {
                PatientIdentityNumber = patient.IdentityNumber,
                StaffIdentityNumber = staffAssignment.StaffIdentityNumber,
                AssignedAt = staffAssignment.AssignedAt,
                PerformedBy = performedBy
            };

            await staffAssignmentPublisher.PublishAssignPatientStaff(dto);
        }

        private async Task PublishStaffUnassignAsync(
            Patient patient,
            PatientStaffAssignment staffAssignment,
            string performedBy)
        {
            var dto = new PatientStaffRemove
            {
                PatientIdentityNumber = patient.IdentityNumber,
                StaffIdentityNumber = staffAssignment.StaffIdentityNumber,
                UnassignedAt = staffAssignment.UnassignedAt ?? DateTime.UtcNow,
                PerformedBy = performedBy
            };

            await staffAssignmentPublisher.PublishUnassignPatientStaff(dto);
        }
        #endregion
    }
}
