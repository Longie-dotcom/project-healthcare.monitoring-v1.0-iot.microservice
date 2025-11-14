using Application.DTO;
using Application.Interface.IService;
using HCM.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        #region Attributes
        private readonly IUserService userService;
        #endregion

        #region Properties
        #endregion

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        #region Methods
        [AuthorizePrivilege("ViewUser")]
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserDTO>> GetByIdAsync(
            Guid userId)
        {
            var user = await userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [AuthorizePrivilege("ViewUser")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(
            [FromQuery] string sortBy,
            [FromQuery] QueryUserDTO dto)
        {
            var users = await userService.GetUsersAsync(sortBy, dto);
            return Ok(users);
        }

        [AuthorizePrivilege("CreateUser")]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(
            [FromBody] UserCreateDTO dto)
        {
            var user = await userService.CreateUserAsync(dto);
            return Ok(user);
        }

        [AuthorizePrivilege("ModifyUser")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(
            Guid id, [FromBody] UserUpdateDTO dto)
        {
            var updatedUser = await userService.UpdateUserAsync(id, dto);
            return Ok(updatedUser);
        }

        [AuthorizePrivilege("DeleteUser")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(
            Guid id, [FromBody] UserDeleteDTO dto)
        {
            await userService.DeleteUserAsync(id, dto);
            return Ok(new { message = "Delete user successfully" });
        }

        [AuthorizePrivilege("ChangePassword")]
        [HttpPut("{id:guid}/change-password")]
        public async Task<IActionResult> ChangePassword(
            Guid id, [FromBody] ChangePasswordDTO dto)
        {
            await userService.ChangePasswordAsync(id, dto);
            return Ok(new { message = "Password changed successfully" });
        }
        #endregion
    }
}
