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
    public class EdgeDeviceController : ControllerBase
    {
        #region Attributes
        private readonly IEdgeDeviceService edgeDeviceService;
        #endregion

        #region Properties
        #endregion

        public EdgeDeviceController(IEdgeDeviceService edgeDeviceService)
        {
            this.edgeDeviceService = edgeDeviceService;
        }

        #region Methods
        [AuthorizePrivilege("ViewDevice")]
        [HttpGet("{edgeDeviceId:guid}")]
        public async Task<ActionResult<EdgeDeviceDTO>> GetByIdAsync(Guid edgeDeviceId)
        {
            var device = await edgeDeviceService.GetByIdAsync(edgeDeviceId);
            return Ok(device);
        }

        [AuthorizePrivilege("ViewDevice")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EdgeDeviceDTO>>> GetAllAsync(
            [FromQuery] QueryEdgeDeviceDTO queryDto,
            [FromQuery] string sort = "")
        {
            var devices = await edgeDeviceService.GetAllAsync(queryDto, sort);
            return Ok(devices);
        }

        [AuthorizePrivilege("CreateDevice")]
        [HttpPost]
        public async Task<ActionResult<EdgeDeviceDTO>> CreateAsync([FromBody] EdgeDeviceCreateDTO dto)
        {
            var device = await edgeDeviceService.CreateAsync(dto);
            return Ok(device);
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpPut("{edgeDeviceId:guid}")]
        public async Task<ActionResult<EdgeDeviceDTO>> UpdateAsync(
            Guid edgeDeviceId, [FromBody] EdgeDeviceUpdateDTO dto)
        {
            var updatedDevice = await edgeDeviceService.UpdateAsync(edgeDeviceId, dto);
            return Ok(updatedDevice);
        }

        [AuthorizePrivilege("DeleteDevice")]
        [HttpDelete("{edgeDeviceId:guid}")]
        public async Task<IActionResult> DeleteAsync(
            Guid edgeDeviceId, [FromBody] EdgeDeviceDeleteDTO dto)
        {
            await edgeDeviceService.DeleteAsync(edgeDeviceId, dto);
            return Ok(new { message = "Device deleted successfully" });
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpPost("controller")]
        public async Task<IActionResult> AssignControllerAsync(
            [FromBody] ControllerCreateDTO dto)
        {
            await edgeDeviceService.AssignControllerAsync(dto);
            return Ok(new { message = "Controller assigned successfully" });
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpPost("sensor")]
        public async Task<IActionResult> AssignSensorAsync(
            [FromBody] SensorCreateDTO dto)
        { 
            await edgeDeviceService.AssignSensorAsync(dto);
            return Ok(new { message = "Sensor assigned successfully" });
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpDelete("controller")]
        public async Task<IActionResult> UnassignControllerAsync(
            [FromBody] ControllerDeleteDTO dto)
        {
            await edgeDeviceService.UnassignControllerAsync(dto);
            return Ok(new { message = "Controller unassigned successfully" });
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpDelete("sensor")]
        public async Task<IActionResult> UnassignSensorAsync(
            [FromBody] SensorDeleteDTO dto)
        {
            await edgeDeviceService.UnassignSensorAsync(dto);
            return Ok(new { message = "Sensor unassigned successfully" });
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpPost("reactivate/edge/{edgeKey}")]
        public async Task<IActionResult> ReactivateEdgeAsync(
            string edgeKey, [FromQuery] string performedBy)
        {
            await edgeDeviceService.ReactivateEdgeDeviceAsync(edgeKey, performedBy);
            return Ok(new { message = "EdgeDevice reactivated successfully" });
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpPost("reactivate/controller/{controllerKey}")]
        public async Task<IActionResult> ReactivateControllerAsync(
            string controllerKey, [FromQuery] string performedBy)
        {
            await edgeDeviceService.ReactivateControllerAsync(controllerKey, performedBy);
            return Ok(new { message = "Controller reactivated successfully" });
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpPost("reactivate/sensor/{sensorKey}")]
        public async Task<IActionResult> ReactivateSensorAsync(
            string sensorKey, [FromQuery] string performedBy)
        {
            await edgeDeviceService.ReactivateSensorAsync(sensorKey, performedBy);
            return Ok(new { message = "Sensor reactivated successfully" });
        }
        #endregion
    }
}
