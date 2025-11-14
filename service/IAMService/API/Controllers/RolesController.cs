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
    public class RolesController : ControllerBase
    {
        #region Attributes
        private readonly IRoleService roleService;
        #endregion

        #region Properties
        #endregion

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        #region Methods
        [AuthorizePrivilege("ViewRole")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAllRoles()
        {
            var roles = await roleService.GetRoleListAsync();
            return Ok(roles);
        }

        [AuthorizePrivilege("ViewRole")]
        [HttpGet("{roleId:guid}")]
        public async Task<ActionResult<RoleDTO>> GetRoleById(
            Guid roleId)
        {
            var role = await roleService.GetRoleByIdAsync(roleId);
            return Ok(role);
        }

        [AuthorizePrivilege("CreateRole")]
        [HttpPost]
        public async Task<ActionResult<RoleDTO>> CreateRole(
            [FromBody] RoleCreateDTO dto)
        {
            var role = await roleService.CreateRoleAsync(dto);
            return CreatedAtAction(nameof(GetRoleById), new { roleId = role.RoleID }, role);
        }

        [AuthorizePrivilege("UpdateRole")]
        [HttpPut("{roleId:guid}")]
        public async Task<ActionResult<RoleDTO>> UpdateRole(
            Guid roleId, [FromBody] RoleUpdateDTO dto)
        {
            var updated = await roleService.UpdateRoleAsync(roleId, dto);
            return Ok(updated);
        }

        [AuthorizePrivilege("DeleteRole")]
        [HttpDelete("{roleId:guid}")]
        public async Task<IActionResult> DeleteRole(
            Guid roleId, [FromBody] UserDeleteDTO dto)
        {
            await roleService.DeleteRoleAsync(roleId, dto);
            return Ok(new { message = "Role deleted successfully" });
        }
        #endregion
    }
}
