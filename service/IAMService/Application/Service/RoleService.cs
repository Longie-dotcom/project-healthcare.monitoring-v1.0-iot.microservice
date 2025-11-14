using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;

namespace Infrastructure.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<RoleDetailDTO> GetRoleByIdAsync(Guid roleId)
        {
            // Get role
            var role = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetByDetailByIdAsync(roleId);
            if (role == null)
                throw new RoleGuidNotFound(roleId);

            var dto = mapper.Map<RoleDetailDTO>(role);
            dto.Privileges = role.RolePrivileges.Select(
                p => mapper.Map<PrivilegeDTO>(p.Privilege)).ToList();

            return dto;
        }

        public async Task<IEnumerable<RoleDTO>> GetRoleListAsync()
        {
            var roles = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetRolesWithFilterAsync();
            var list = new List<RoleDTO>();
            foreach (var role in roles)
            {
                var roleDto = mapper.Map<RoleDTO>(role);
                roleDto.PrivilegeID = role.RolePrivileges.Select( p => p.PrivilegeID ).ToList();
                list.Add(roleDto);
            }

            return list;
        }

        public async Task<RoleDTO> CreateRoleAsync(RoleCreateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var existedRoles = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetAllAsync();

            // Validate role code duplication
            var existedCode = existedRoles.Where(r => r.Code == dto.RoleCode);
            if (existedCode.Any())
                throw new RoleCodeAlreadyExists(dto.RoleCode);
            
            // Create base role
            var newRole = new Role
            (
                Guid.NewGuid(),
                dto.Name,
                dto.RoleCode,
                dto.Description
            );

            // Link privileges
            foreach (var privilegeId in dto.PrivilegeID)
            {
                newRole.AddPrivilege(privilegeId);
            }

            unitOfWork
                .GetRepository<IRoleRepository>()
                .Add(newRole);
           
            await unitOfWork.CommitAsync(dto.PerformedBy);

            var result = mapper.Map<RoleDTO>(newRole);
            result.PrivilegeID = dto.PrivilegeID;

            return result;
        }

        public async Task<RoleDTO> UpdateRoleAsync(Guid roleId, RoleUpdateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var repo = unitOfWork.GetRepository<IRoleRepository>();
            var existing = await repo.GetByIdAsync(roleId);
            if (existing == null)
                throw new RoleGuidNotFound(roleId);

            existing.UpdateName(dto.Name);
            existing.UpdateDescription(dto.Description);

            // Update privileges (many to many)
            await unitOfWork.GetRepository<IRoleRepository>()
                .UpdateRolePrivilegesAsync(roleId, dto.PrivilegeID);

            await unitOfWork.CommitAsync(dto.PerformedBy);

            var result = mapper.Map<RoleDTO>(existing);
            result.PrivilegeID = existing.RolePrivileges
                .Where(rp => rp.IsActive)
                .Select(rp => rp.PrivilegeID)
                .ToList();

            return result;
        }

        public async Task DeleteRoleAsync(Guid roleId, UserDeleteDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var role = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetByIdAsync(roleId);
            if (role == null)
                throw new RoleGuidNotFound(roleId);

            unitOfWork
                .GetRepository<IRoleRepository>()
                .Remove(roleId);

            await unitOfWork.CommitAsync(dto.PerformBy);
        }
        #endregion
    }
}
