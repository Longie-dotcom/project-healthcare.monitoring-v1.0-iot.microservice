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
    public class PrivilegesController : ControllerBase
    {
        #region Attributes
        private readonly IPrivilegeService privilegeService;
        #endregion

        #region Properties
        #endregion

        public PrivilegesController(IPrivilegeService privilegeService)
        {
            this.privilegeService = privilegeService;
        }

        #region Methods
        [AuthorizePrivilege("ViewPrivilege")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrivilegeDTO>>> GetAllPrivileges()
        {
            var Privileges = await privilegeService.GetPrivilegesAsync();
            return Ok(Privileges);
        }

        [AuthorizePrivilege("ViewPrivilege")]
        [HttpGet("{PrivilegeId:guid}")]
        public async Task<ActionResult<PrivilegeDTO>> GetPrivilegeById(
            Guid PrivilegeId)
        {
            var Privilege = await privilegeService.GetPrivilegeByIdAsync(PrivilegeId);
            return Ok(Privilege);
        }

        [AuthorizePrivilege("CreatePrivilege")]
        [HttpPost]
        public async Task<ActionResult<PrivilegeDTO>> CreatePrivilege(
            [FromBody] PrivilegeCreateDTO dto)
        {
            var Privilege = await privilegeService.CreatePrivilegeAsync(dto);
            return CreatedAtAction(nameof(GetPrivilegeById), new { PrivilegeId = Privilege.PrivilegeID }, Privilege);
        }

        [AuthorizePrivilege("UpdatePrivilege")]
        [HttpPut("{privilegeId:guid}")]
        public async Task<ActionResult<PrivilegeDTO>> UpdatePrivilege(
            Guid privilegeId, [FromBody] PrivilegeUpdateDTO dto)
        {
            var updated = await privilegeService.UpdatePrivilegeAsync(privilegeId, dto);
            return Ok(updated);
        }

        [AuthorizePrivilege("DeletePrivilege")]
        [HttpDelete("{privilegeId:guid}")]
        public async Task<IActionResult> DeletePrivilege(
            Guid privilegeId, [FromBody] UserDeleteDTO dto)
        {
            await privilegeService.DeletePrivilegeAsync(privilegeId, dto);
            return Ok(new { message = "Privilege deleted successfully" });
        }
        #endregion
    }
}
