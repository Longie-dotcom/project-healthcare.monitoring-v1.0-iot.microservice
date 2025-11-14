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
        private readonly IEdgeDeviceService edgeDeviceService;

        public EdgeDeviceController(IEdgeDeviceService edgeDeviceService)
        {
            this.edgeDeviceService = edgeDeviceService;
        }

        #region EdgeDevice Endpoints
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
        [HttpPost("{edgeDeviceId:guid}/deactivate")]
        public async Task<IActionResult> DeactivateAsync(Guid edgeDeviceId, [FromBody] EdgeDeviceDeactiveDTO dto)
        {
            await edgeDeviceService.DeactiveAsync(edgeDeviceId, dto);
            return Ok(new { message = "Edge device deactivated successfully" });
        }
        #endregion

        #region Controller Endpoints
        [AuthorizePrivilege("UpdateDevice")]
        [HttpPost("controller")]
        public async Task<IActionResult> AssignControllerAsync([FromBody] ControllerCreateDTO dto)
        {
            await edgeDeviceService.AssignControllerAsync(dto);
            return Ok(new { message = "Controller assigned successfully" });
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpPut("controller")]
        public async Task<ActionResult<ControllerDTO>> UpdateControllerAsync([FromBody] ControllerUpdateDTO dto)
        {
            var updatedController = await edgeDeviceService.UpdateControllerAsync(dto);
            return Ok(updatedController);
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpDelete("controller")]
        public async Task<IActionResult> UnassignControllerAsync([FromBody] ControllerUnassignDTO dto)
        {
            await edgeDeviceService.UnassignControllerAsync(dto);
            return Ok(new { message = "Controller unassigned successfully" });
        }
        #endregion

        #region Sensor Endpoints
        [AuthorizePrivilege("UpdateDevice")]
        [HttpPost("sensor")]
        public async Task<IActionResult> AssignSensorAsync([FromBody] SensorCreateDTO dto)
        {
            await edgeDeviceService.AssignSensorAsync(dto);
            return Ok(new { message = "Sensor assigned successfully" });
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpPut("sensor")]
        public async Task<ActionResult<SensorDTO>> UpdateSensorAsync([FromBody] SensorUpdateDTO dto)
        {
            var updatedSensor = await edgeDeviceService.UpdateSensorAsync(dto);
            return Ok(updatedSensor);
        }

        [AuthorizePrivilege("UpdateDevice")]
        [HttpDelete("sensor")]
        public async Task<IActionResult> UnassignSensorAsync([FromBody] SensorUnassignDTO dto)
        {
            await edgeDeviceService.UnassignSensorAsync(dto);
            return Ok(new { message = "Sensor unassigned successfully" });
        }
        #endregion
    }
}
