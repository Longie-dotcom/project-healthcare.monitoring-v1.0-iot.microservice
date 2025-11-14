using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.IRepository;

namespace Infrastructure.Service
{
    public class PrivilegeService : IPrivilegeService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        #endregion

        public PrivilegeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<PrivilegeDTO?> GetPrivilegeByIdAsync(Guid privilegeId)
        {
            var privilege = await unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .GetByIdAsync(privilegeId);
            return mapper.Map<PrivilegeDTO?>(privilege);
        }

        public async Task<IEnumerable<PrivilegeDTO>> GetPrivilegesAsync()
        {
            var privileges = await unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .GetAllAsync();
            var list = new List<PrivilegeDTO>();
            foreach (var privilege in privileges)
            {
                PrivilegeDTO privilegeDTO = new PrivilegeDTO()
                {
                    Description = privilege.Description,
                    Name = privilege.Name,
                    PrivilegeID = privilege.PrivilegeID
                };
                list.Add(privilegeDTO);
            }

            if (list.Count <= 0)
                throw new PrivilegeNotFound("No privileges found.");

            return list;
        }

        public async Task<PrivilegeDTO> CreatePrivilegeAsync(PrivilegeCreateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IPrivilegeRepository>();

            if (await repo.ExistsByNameAsync(dto.Name))
                throw new PrivilegeAlreadyExists($"Privilege name '{dto.Name}' already exists.");

            await unitOfWork.BeginTransactionAsync();

            var privilege = new Privilege(Guid.NewGuid(), dto.Name, dto.Description);
            repo.Add(privilege);

            await unitOfWork.CommitAsync(dto.PerformedBy);

            return mapper.Map<PrivilegeDTO>(privilege);
        }

        public async Task<PrivilegeDTO> UpdatePrivilegeAsync(Guid privilegeId, PrivilegeUpdateDTO dto)
        {
            var repo = unitOfWork.GetRepository<IPrivilegeRepository>();
            var privilege = await repo.GetByIdAsync(privilegeId);

            if (privilege == null)
                throw new PrivilegeNotFound($"Privilege with ID '{privilegeId}' not found.");

            if (await repo.ExistsByNameExceptIdAsync(dto.Name, privilegeId))
                throw new PrivilegeAlreadyExists($"Privilege name '{dto.Name}' already exists.");

            await unitOfWork.BeginTransactionAsync();

            privilege.UpdateNameAndDescription(dto.Name, dto.Description);

            repo.Update(privilegeId, privilege);

            await unitOfWork.CommitAsync(dto.PerformedBy);

            return mapper.Map<PrivilegeDTO>(privilege);
        }

        public async Task DeletePrivilegeAsync(Guid privilegeId, UserDeleteDTO dto)
        {
            var repo = unitOfWork.GetRepository<IPrivilegeRepository>();
            var privilege = await repo.GetByIdAsync(privilegeId);

            if (privilege == null)
                throw new PrivilegeNotFound($"Privilege with ID '{privilegeId}' not found.");

            await unitOfWork.BeginTransactionAsync();

            repo.Remove(privilegeId);

            await unitOfWork.CommitAsync(dto.PerformBy);
        }
        #endregion
    }
}
