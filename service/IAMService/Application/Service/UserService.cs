using Application.ApplicationException;
using Application.DTO;
using Application.Enum;
using Application.Interface.IMessagePublisher;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.DomainException;
using Domain.IRepository;
using System.Data;

namespace Application.Service
{
    public class UserService : IUserService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUpdateUserPublisher updateUserPublisher;
        private readonly IDeleteUserPublisher deleteUserPublisher;
        #endregion

        #region Properties
        #endregion

        public UserService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            IUpdateUserPublisher updateUserPublisher,
            IDeleteUserPublisher deleteUserPublisher)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.updateUserPublisher = updateUserPublisher;
            this.deleteUserPublisher = deleteUserPublisher;
        }

        #region Methods
        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDTO dto)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdAsync(userId);

            if (user == null)
                throw new UserNotFound();

            if (dto.NewPassword != dto.NewConfirmedPassword)
                throw new InvalidChangePassword(
                    "New password and confirmed password are not matched.");

            if (!user.Password.Verify(dto.OldPassword))
                throw new InvalidChangePassword(
                    "Old password does not matched.");

            await unitOfWork.BeginTransactionAsync();
            user.ChangePassword(dto.NewPassword);
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(userId, user);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task<UserDTO> GetByEmailAsync(string email)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByEmailAsync(email);
            if (user == null)
                throw new UserNotFound();
            return mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetByIdentityNumberAsync(string identityNumber)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberAsync(identityNumber);
            if (user == null)
                throw new UserNotFound();
            return mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetByPhoneAsync(string phone)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByPhoneAsync(phone);
            if (user == null)
                throw new UserNotFound();
            return mapper.Map<UserDTO>(user);
        }

        public async Task<UserDetailDTO> GetUserByIdAsync(Guid userId)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByDetailByIdAsync(userId);

            if (user == null)
                throw new UserNotFound();
            var dto = mapper.Map<UserDetailDTO>(user);

            // Get privileges of each role
            var roleDtos = new List<UserRoleDTO>();
            foreach (var userRole in user.UserRoles)
            {
                var roleDto = mapper.Map<RoleDetailDTO>(userRole.Role);
                roleDto.Privileges =
                    userRole.Role.RolePrivileges.Select(
                    p => mapper.Map<PrivilegeDTO>(p.Privilege)).ToList();

                roleDtos.Add(new UserRoleDTO()
                {
                    Role = roleDto,
                    IsActive = userRole.IsActive,
                });
            }
            dto.UserRoles = roleDtos;

            // Get granted privileges of user
            var privilegeDtos = new List<UserPrivilegeDTO>();
            foreach (var userPrivilege in user.UserPrivileges)
            {
                var privilegeDto = mapper.Map<PrivilegeDTO>(userPrivilege.Privilege);

                privilegeDtos.Add(new UserPrivilegeDTO()
                {
                    Privilege = privilegeDto,
                    IsGranted = userPrivilege.IsGranted,
                });
            }
            dto.UserPrvileges = privilegeDtos;

            return dto;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync(
            string sortBy,
            QueryUserDTO dto)
        {
            var users = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetUsersWithFilterAsync(
                dto.PageIndex,
                dto.PageLength,
                dto.Search,
                dto.Gender,
                dto.IsActive,
                dto.DateOfBirthFrom,
                dto.DateOfBirthTo);

            if (!users.Any())
                throw new UserNotFound();

            users = sortBy switch
            {
                SortKeyword.SORT_BY_EMAIL => users.OrderBy(u => u.Email),
                SortKeyword.SORT_BY_AGE => users.OrderBy(u => u.Age),
                SortKeyword.SORT_BY_GENDER => users.OrderBy(u => u.Gender),
                SortKeyword.SORT_BY_PHONE => users.OrderBy(u => u.Phone),
                SortKeyword.SORT_BY_ADDRESS => users.OrderBy(u => u.Address),
                SortKeyword.SORT_BY_IDENTITY => users.OrderBy(u => u.IdentityNumber),
                _ => users.OrderBy(u => u.FullName)
            };

            return users.Select(user => mapper.Map<UserDTO>(user));
        }

        public async Task<UserDTO> CreateUserAsync(UserCreateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var existingByIdentity = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberTrackingAsync(dto.IdentityNumber);
            if (existingByIdentity != null)
            {
                if (existingByIdentity.IsActive)
                {
                    throw new UserAlreadyExists(
                        "Identity number already registered.");
                }
                else
                {
                    existingByIdentity.UpdateActive(true);
                    await unitOfWork.CommitAsync(dto.PerformedBy);
                    return mapper.Map<UserDTO>(existingByIdentity);
                }
            }

            var existingByEmail = await unitOfWork
                .GetRepository<IUserRepository>()
                .ExistsByEmailAsync(dto.Email);
            if (existingByEmail)
                throw new UserAlreadyExists(
                    "Email already registered.");

            var existingByPhone = await unitOfWork
                .GetRepository<IUserRepository>()
                .ExistsByPhoneAsync(dto.PhoneNumber);
            if (existingByPhone)
                throw new UserAlreadyExists(
                    "Phone number already registered.");

            if (dto.RoleCodes == null || dto.RoleCodes.Count == 0)
                throw new InvalidUserAggregateException(
                    "User must have at least one role.");

            var user = new User(
                userID: Guid.NewGuid(),
                email: dto.Email,
                fullName: dto.FullName,
                password: dto.Password,
                dob: dto.DateOfBirth,
                address: dto.Address,
                gender: dto.Gender,
                phone: dto.PhoneNumber,
                identityNumber: dto.IdentityNumber,
                isActive: true
            );

            foreach (var code in dto.RoleCodes)
            {
                var role = await unitOfWork
                .GetRepository<IRoleRepository>()
                    .GetByCodeAsync(code);
                if (role == null)
                    throw new InvalidUserAggregateException(
                        $"Role code '{code}' does not exist.");
                user.AddRole(role.RoleID);
            }

            unitOfWork
                .GetRepository<IUserRepository>()
                .Add(user);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            return mapper.Map<UserDTO>(user);
        }

        public async Task DeleteUserAsync(Guid userId, UserDeleteDTO dto)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFound();

            await unitOfWork.BeginTransactionAsync();
            user.UpdateActive(false);
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(userId, user);
            await unitOfWork.CommitAsync(dto.PerformBy);

            await deleteUserPublisher.PublishAsync(new SyncDeleteUserDTO()
            {
                IdentityNumber = user.IdentityNumber,
                PerformedBy = dto.PerformBy
            });
        }

        public async Task<UserDTO> UpdateUserAsync(Guid userId, UserUpdateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdAsync(userId)
                ?? throw new UserNotFound();

            // Check if another user contains the email (excluding current user)
            var existingByEmail = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetAllAsync();
            if (existingByEmail.Any(u => u.UserID != userId && u.Email.Contains(dto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new UserAlreadyExists("Email already registered.");
            }

            // Check if another user contains the phone number (excluding current user)
            if (existingByEmail.Any(u => u.UserID != userId && u.Phone.Contains(dto.PhoneNumber)))
            {
                throw new UserAlreadyExists("Phone number already registered.");
            }

            // Update fields
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.UpdateFullName(dto.FullName);

            if (!string.IsNullOrWhiteSpace(dto.Address))
                user.UpdateAddress(dto.Address);

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.UpdatePhone(dto.PhoneNumber);

            if (!string.IsNullOrWhiteSpace(dto.Gender))
                user.UpdateGender(dto.Gender);

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.UpdateEmail(dto.Email);

            if (dto.DateOfBirth != null)
                user.UpdateDob(dto.DateOfBirth.Value);

            var privilegeUpdates = dto.UserPrivilegeUpdateDTOs?
                .Select(p => (p.IsGranted, p.PrivilegeID))
                .ToList() ?? new List<(bool IsGranted, Guid PrivilegeID)>();

            await unitOfWork
                .GetRepository<IUserRepository>()
                .UpdateUserPrivilegesAsync(userId, privilegeUpdates);

            // Update user roles
            if (dto.UserRoleUpdateDTOs?.Any() == true)
            {
                var roleUpdates = dto.UserRoleUpdateDTOs
                    .Select(r => (r.IsActive, r.RoleID))
                    .ToList();

                await unitOfWork
                    .GetRepository<IUserRepository>()
                    .UpdateUserRolesAsync(userId, roleUpdates);
            }

            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            await updateUserPublisher.PublishAsync(new SyncUpdateUserDTO()
            {
                IdentityNumber = user.IdentityNumber,
                PerformedBy = dto.PerformedBy,
                Email = dto.Email,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                FullName = dto.FullName,
                Gender = dto.Gender,
                Phone = dto.PhoneNumber,
            });

            return mapper.Map<UserDTO>(user);
        }

        public async Task SyncUserUpdate(SyncUpdateUserDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberTrackingAsync(dto.IdentityNumber)
                ?? throw new UserNotFound();

            // IAM does not accept updated field phone and email from other services
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.UpdateFullName(dto.FullName);

            if (!string.IsNullOrWhiteSpace(dto.Address))
                user.UpdateAddress(dto.Address);

            if (!string.IsNullOrWhiteSpace(dto.Gender))
                user.UpdateGender(dto.Gender);

            if (dto.DateOfBirth != null)
                user.UpdateDob(dto.DateOfBirth ?? DateTime.MinValue);

            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }
        #endregion
    }
}
