using Application.DTO;
using Application.Interface.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        #region Attributes
        private readonly IAuthService authService;
        #endregion

        #region Properties
        #endregion

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        #region Methods
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(
            [FromBody] LoginDTO dto)
        {
            var authResponse = await authService.Login(dto);
            return Ok(authResponse);
        }
        
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordDTO dto)
        {
            await authService.ForgotPasswordAsync(dto.Email);
            return Ok(new { message = "A password reset link has been sent." });
        }
        
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDTO dto)
        {
            await authService.ResetPasswordAsync(dto);
            return Ok(new { message = "Password has been reset successfully." });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenDTO dto)
        {
            var authResponse = await authService.RefreshAccessToken(dto);
            return Ok(authResponse);
        }

        [AllowAnonymous]
        [HttpGet("logout/{email}")]
        public async Task<IActionResult> Logout(
            string email)
        {
            await authService.Logout(email);
            return Ok(new { message = "User logout successfully." });
        }
        #endregion
    }
}
