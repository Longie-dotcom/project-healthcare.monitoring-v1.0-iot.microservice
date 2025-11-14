using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.IRepository;
using System.Security.Claims;

namespace Application.Service
{
    public class AuthService : IAuthService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;
        #endregion

        #region Properties
        #endregion

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IEmailService emailService,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.tokenService = tokenService;
            this.emailService = emailService;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<TokenDTO> Login(LoginDTO loginDTO)
        {
            // Get repositories
            var userRepo = unitOfWork.GetRepository<IUserRepository>();
            var roleRepo = unitOfWork.GetRepository<IRoleRepository>();
            var refreshTokenRepo = unitOfWork.GetRepository<IRefreshTokenRepository>();

            #region Verify user
            // Get user
            var user = await userRepo.GetByEmailAsync(loginDTO.Email);
            if (user == null)
                throw new UserNotFound();
            if (!user.IsActive)
                throw new UserNotFound();

            // Verify password
            if (!user.Password.Verify(loginDTO.Password))
                throw new InvalidPassword();
            #endregion

            #region Verify user role
            // Get role
            var role = await roleRepo.GetByCodeAsync(loginDTO.RoleCode);
            if (role == null)
                throw new RoleCodeNotFound(loginDTO.RoleCode);

            // Get roles of this user
            var userRoles = user.UserRoles;
            if (userRoles == null || !userRoles.Any())
                throw new InvalidRole();

            // Match role to user's assigned roles
            var matchedUserRole = userRoles.FirstOrDefault(
                ur => ur.RoleID == role.RoleID && ur.IsActive);
            if (matchedUserRole == null)
                throw new InvalidRole();
            #endregion

            // Get privileges
            var privileges = user.GetEffectivePrivileges(role.RoleID);

            // Generate access token
            var accessToken = tokenService.GenerateToken(
                privileges,
                user.IdentityNumber,
                user.Email,
                user.FullName,
                role.Code
            );

            // Generate refresh token
            await unitOfWork.BeginTransactionAsync();
            var refreshToken = await refreshTokenRepo.AddTokenAsync(user.UserID);
            await unitOfWork.CommitAsync(user.Email);

            // Return tokens
            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByEmailAsync(email);
            if (user == null)
                throw new UserEmailNotFound(email);
            var token = tokenService.GeneratePasswordResetToken(user, 10);
            var client = Environment.GetEnvironmentVariable("IAM_CLIENT_SIDE");
            var resetLink = $"{client}/reset-password?token={token}";
            await emailService.SendPasswordResetEmailAsync(email, resetLink);
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new InvalidResetPassword("Reset password not matched.");

            var principal = tokenService.GetPrincipalFromToken(dto.ResetToken);
            if (principal == null || principal.FindFirst("Purpose")?.Value != "PasswordReset")
                throw new InvalidResetPassword("Invalid or expired token.");

            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new InvalidResetPassword("Invalid token payload.");

            await unitOfWork.BeginTransactionAsync();

            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFound();

            user.ChangePassword(dto.NewPassword);
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);

            await unitOfWork.CommitAsync();
        }

        public async Task<string> RefreshAccessToken(RefreshTokenDTO dto)
        {
            #region Verify user and refresh token
            // Verify existed user
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByEmailAsync(dto.Email);
            if (user == null)
                throw new UserNotFound();

            // Verify refresh token
            var refreshedToken = await unitOfWork
                .GetRepository<IRefreshTokenRepository>()
                .GetByTokenAsync(dto.RefreshToken);
            if (refreshedToken == null)
                throw new InvalidTokenException("Refreshed token is expired or not found");
            #endregion

            #region Verify current login role
            // Get role
            var role = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetByCodeAsync(dto.RoleCode);
            if (role == null)
                throw new RoleCodeNotFound(dto.RoleCode);

            // Get roles of this user
            var userRoles = user.UserRoles;
            if (userRoles == null || !userRoles.Any())
                throw new InvalidRole();

            // Match role to user's assigned roles
            var matchedUserRole = userRoles.FirstOrDefault(ur => ur.RoleID == role.RoleID);
            if (matchedUserRole == null)
                throw new InvalidRole();
            #endregion

            // Get role privileges
            var privileges = user.GetEffectivePrivileges(role.RoleID);

            // Generate access token
            var accessToken = tokenService.GenerateToken(
                privileges,
                user.IdentityNumber,
                user.Email,
                user.FullName,
                role.Code
            );

            return accessToken;
        }

        public async Task Logout(string email)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByEmailAsync(email);
            if (user == null)
                throw new UserNotFound();
            
            await unitOfWork.BeginTransactionAsync();
            await unitOfWork
                .GetRepository<IRefreshTokenRepository>()
                .DeleteTokenAsync(user.UserID);
            await unitOfWork.CommitAsync();
        }
        #endregion
    }
}
