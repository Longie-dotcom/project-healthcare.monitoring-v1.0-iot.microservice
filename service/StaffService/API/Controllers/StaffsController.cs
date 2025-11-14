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
    public class StaffsController : ControllerBase
    {
        #region Attributes
        private readonly IStaffService StaffService;
        #endregion

        #region Properties
        #endregion

        public StaffsController(IStaffService StaffService)
        {
            this.StaffService = StaffService;
        }

        #region Methods
        [AuthorizePrivilege("ViewStaff")]
        [HttpGet("{StaffId:guid}")]
        public async Task<ActionResult<StaffDTO>> GetByIdAsync(
            Guid StaffId)
        {
            var Staff = await StaffService.GetByIdAsync(StaffId);
            return Ok(Staff);
        }

        [AuthorizePrivilege("ViewStaff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffDTO>>> GetStaffs(
            [FromQuery] string sortBy,
            [FromQuery] QueryStaffDTO dto)
        {
            var Staffs = await StaffService.GetAllAsync(dto, sortBy);
            return Ok(Staffs);
        }

        [AuthorizePrivilege("CreateStaff")]
        [HttpPost]
        public async Task<ActionResult<StaffDTO>> CreateStaff(
            [FromBody] StaffCreateDTO dto)
        {
            var Staff = await StaffService.CreateAsync(dto);
            return Ok(Staff);
        }

        [AuthorizePrivilege("UpdateStaff")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<StaffDTO>> UpdateStaff(
            Guid id, [FromBody] StaffUpdateDTO dto)
        {
            var updatedStaff = await StaffService.UpdateAsync(id, dto);
            return Ok(updatedStaff);
        }

        [AuthorizePrivilege("DeleteStaff")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteStaff(
            Guid id, [FromBody] StaffDeleteDTO dto)
        {
            await StaffService.DeleteAsync(id, dto);
            return Ok(new { message = "Delete Staff successfully" });
        }
        #endregion
    }
}
