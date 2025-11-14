using Application.DTO;

namespace Application.Interface.IService
{
    public interface IAuthService
    {
        Task<TokenDTO> Login(LoginDTO loginDTO);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDTO dto);
        Task<string> RefreshAccessToken(RefreshTokenDTO dto);
        Task Logout(string email);
    }
}
