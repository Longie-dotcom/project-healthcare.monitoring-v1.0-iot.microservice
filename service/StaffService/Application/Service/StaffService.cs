using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IGrpc;
using Application.Interface.IMessagePublisher;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using HCM.CodeFormatter;
using HCM.MessageBrokerDTOs;
using System.Data;

namespace Application.Service
{
    public class StaffService : IStaffService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IIAMGrpcClient iAMGrpcClient;
        private readonly IUpdateUserPublisher updateUserPublisher;
        #endregion

        #region Properties
        #endregion

        public StaffService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IIAMGrpcClient iAMGrpcClient,
            IUpdateUserPublisher updateUserPublisher)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.iAMGrpcClient = iAMGrpcClient;
            this.updateUserPublisher = updateUserPublisher;
        }

        #region Methods
        public async Task<StaffDTO> GetByIdAsync(Guid staffId)
        {
            var repo = unitOfWork.GetRepository<IStaffRepository>();
            var staff = await repo.GetDetailedByIdAsync(staffId);
            if (staff == null) 
                throw new StaffNotFound(
                    $"Staff with ID:{staffId} is not found");
            return mapper.Map<StaffDTO>(staff);
        }

        public async Task<IEnumerable<StaffDTO>> GetAllAsync(QueryStaffDTO dto, string? sort)
        {
            var repo = unitOfWork.GetRepository<IStaffRepository>();
            var list = await repo.GetAllWithFilter(dto.Search ?? string.Empty, dto.PageIndex, dto.PageLength);

            list = sort?.ToLower() switch
            {
                "ProfessionalTitle" => list.OrderBy(s => s.ProfessionalTitle),
                "Specialization" => list.OrderBy(s => s.Specialization),
                _ => list.OrderBy(s => s.StaffCode)
            };

            if (list == null || !list.Any())
                throw new StaffNotFound("The staff list is empty");

            return mapper.Map<IEnumerable<StaffDTO>>(list);
        }

        public async Task<StaffDTO> CreateAsync(StaffCreateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IStaffRepository>();

            // Check if identity number exists
            var existing = await repo.GetStaffByIdentityNumber(dto.IdentityNumber);
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
                    return mapper.Map<StaffDTO>(existing);
                }
            }

            // Validate staff via IAM
            var user = await iAMGrpcClient.GetUser(dto.IdentityNumber);
            if (user == null)
                throw new UserNotFound(
                    $"User with identity number {dto.IdentityNumber} is not found");

            // Calculate next index
            var allStaff = (await repo.GetAllAsync()).ToList();
            var nextIndex = allStaff.Count + 1;

            // Generate code using central formatter
            var staffCode = CodeFormatter.Format("STF", dto.IdentityNumber, DateTime.UtcNow, nextIndex);

            // Create new staff
            var staff = new Staff(
                Guid.NewGuid(),
                staffCode,
                dto.ProfessionalTitle,
                dto.Specialization,
                dto.AvatarUrl ?? "",
                user.IdentityNumber,
                user.Email,
                user.FullName,
                user.DateOfBirth ?? DateTime.MinValue,
                user.Address,
                user.Gender,
                user.PhoneNumber
            );

            repo.Add(staff);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            return mapper.Map<StaffDTO>(staff);
        }

        public async Task<StaffDTO> UpdateAsync(Guid staffId, StaffUpdateDTO dto)
        {
            var staffRepo = unitOfWork.GetRepository<IStaffRepository>();
            var staff = await staffRepo.GetDetailedByIdAsync(staffId);

            if (staff == null)
                throw new StaffNotFound(
                    $"Staff with ID:{staffId} is not found.");

            // ===============================
            // Update simple fields
            // ===============================
            if (!string.IsNullOrEmpty(dto.AvatarUrl))
                staff.UpdateAvatar(dto.AvatarUrl);

            if (!string.IsNullOrEmpty(dto.Specialization))
                staff.UpdateSpecialization(dto.Specialization);

            if (!string.IsNullOrEmpty(dto.ProfessionalTitle))
                staff.UpdateProfessionalTitle(dto.ProfessionalTitle);

            if (!string.IsNullOrEmpty(dto.FullName))
                staff.UpdateFullName(dto.FullName);

            if (!string.IsNullOrEmpty(dto.Gender))
                staff.UpdateGender(dto.Gender);

            if (!string.IsNullOrEmpty(dto.Address))
                staff.UpdateAddress(dto.Address);

            if (dto.DateOfBirth != null)
                staff.UpdateDob(dto.DateOfBirth ?? DateTime.MinValue);

            // ===============================
            // Manually construct child entities
            // ===============================
            if (dto.StaffLicenses != null)
            {
                // Check for duplicates in the incoming DTO first
                var duplicateInDto = dto.StaffLicenses
                    .GroupBy(l => l.LicenseNumber)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateInDto.Any())
                    throw new LicenseNumberExisted(
                        $"Duplicate license numbers in request: {string.Join(", ", duplicateInDto)}");

                // Map DTO to entity
                var mappedLicenses = dto.StaffLicenses
                    .Select(l => new StaffLicense(
                        Guid.NewGuid(),
                        staffId,
                        l.LicenseNumber,
                        l.LicenseType,
                        l.IssuedBy,
                        l.IssueDate ?? DateTime.UtcNow,
                        l.ExpiryDate ?? DateTime.UtcNow.AddYears(1)
                    ))
                    .ToList();

                // Update child licenses
                await staffRepo.UpdateLicensesAsync(staffId, mappedLicenses);
            }

            if (dto.StaffSchedules != null)
            {
                var mappedSchedules = dto.StaffSchedules
                    .Select(s => new StaffSchedule(
                        Guid.NewGuid(),
                        staffId,
                        s.DayOfWeek,
                        s.ShiftStart ?? TimeSpan.Zero,
                        s.ShiftEnd ?? TimeSpan.Zero,
                        s.IsActive
                    ))
                    .ToList();

                await staffRepo.UpdateSchedulesAsync(staffId, mappedSchedules);
            }

            if (dto.StaffAssignments != null)
            {
                var mappedAssignments = dto.StaffAssignments
                    .Select(a => new StaffAssignment(
                        Guid.NewGuid(),
                        staffId,
                        a.Department,
                        a.Role,
                        a.StartDate ?? DateTime.UtcNow,
                        a.EndDate,
                        a.IsActive
                    ))
                    .ToList();

                await staffRepo.UpdateAssignmentsAsync(staffId, mappedAssignments);
            }

            if (dto.StaffExperiences != null)
            {
                var mappedExperiences = dto.StaffExperiences
                    .Select(e => new StaffExperience(
                        Guid.NewGuid(),
                        staffId,
                        e.Institution,
                        e.Position,
                        e.StartYear ?? DateTime.UtcNow.Year,
                        e.EndYear ?? DateTime.UtcNow.Year,
                        e.Description ?? string.Empty
                    ))
                    .ToList();

                await staffRepo.UpdateExperiencesAsync(staffId, mappedExperiences);
            }

            // ===============================
            // Save all changes
            // ===============================
            await unitOfWork.CommitAsync(dto.PerformedBy);

            // Email and phone can not be updated from staff service
            await updateUserPublisher.PublishAsync(
                new UpdateUser()
                {
                    IdentityNumber = staff.IdentityNumber,
                    PerformedBy = dto.PerformedBy,
                    Address = dto.Address,
                    Dob = dto.DateOfBirth,
                    Name = dto.FullName,
                    Gender = dto.Gender,
                });

            // ===============================
            // Map Staff -> StaffDTO for response
            // ===============================
            return mapper.Map<StaffDTO>(staff);
        }

        public async Task SyncUpdateAsync(UpdateUser dto)
        {
            var staffRepo = unitOfWork.GetRepository<IStaffRepository>();
            var staff = await staffRepo.GetStaffByIdentityNumber(dto.IdentityNumber);

            if (staff == null)
                throw new StaffNotFound(
                    $"Staff with identity number:{dto.IdentityNumber} is not found.");

            if (!string.IsNullOrEmpty(dto.Name))
                staff.UpdateFullName(dto.Name);

            if (!string.IsNullOrEmpty(dto.Gender))
                staff.UpdateGender(dto.Gender);

            if (!string.IsNullOrEmpty(dto.Email))
                staff.UpdateEmail(dto.Email);

            if (!string.IsNullOrEmpty(dto.Address))
                staff.UpdateAddress(dto.Address);

            if (!string.IsNullOrEmpty(dto.Phone))
                staff.UpdatePhone(dto.Phone);

            if (dto.Dob != null)
                staff.UpdateDob(dto.Dob ?? DateTime.MinValue);

            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task DeleteAsync(Guid staffId, StaffDeleteDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IStaffRepository>();
            var staff = await repo.GetDetailedByIdAsync(staffId);
            if (staff == null)
                throw new StaffNotFound(
                    $"Staff with ID:{staffId} is not found.");
            staff.UpdateActive(false);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task SyncDeleteAsync(DeleteUser dto)
        {
            await unitOfWork.BeginTransactionAsync();
            var repo = unitOfWork.GetRepository<IStaffRepository>();
            var staff = await repo.GetStaffByIdentityNumber(dto.IdentityNumber);
            if (staff == null)
                throw new StaffNotFound(
                    $"Staff with identity number:{dto.IdentityNumber} is not found.");
            staff.UpdateActive(false);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }
        #endregion
    }
}
