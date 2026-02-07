using Application.DTO;
using Application.Interface.IService;
using FSA.LaboratoryManagement.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RoomProfilesController : ControllerBase
    {
        #region Attributes
        private readonly IRoomProfileService roomProfileService;
        #endregion

        #region Properties
        #endregion

        public RoomProfilesController(
            IRoomProfileService roomProfileService)
        {
            this.roomProfileService = roomProfileService;
        }

        #region Methods
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomProfileDTO>>> GetRoomProfiles()
        {
            var profiles = await roomProfileService.GetRoomProfilesAsync();
            return Ok(profiles);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ReceiveSensorData([FromBody] RawSensorData data)
        {
            await roomProfileService.ReceiveDataAsync(data);
            return Ok(new { status = "received" });
        }

        [AuthorizePrivilege("ReadOnly")]
        [HttpGet("patient/{patientIdentityNumber}")]
        public async Task<ActionResult<DeviceProfileDTO>> GetMyPatient(string patientIdentityNumber)
        {
            // Extract staff identity from the claims populated by TokenClaimsMiddleware
            var staffIdentityNumber = User.Claims
                .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(staffIdentityNumber))
                return Unauthorized("Cannot determine staff identity from headers.");

            var query = new GetPatientDataDTO()
            {
                StaffIdentityNumber = staffIdentityNumber,
                PatientIdentityNumber = patientIdentityNumber
            };

            var patientData = await roomProfileService.GetPatientData(query);
            return Ok(patientData);
        }

        [AuthorizePrivilege("ReadOnly")]
        [HttpGet("my-patients")]
        public async Task<ActionResult<IEnumerable<StaffAssignedControllerDTO>>> GetMyPatients()
        {
            // Extract staff identity from the claims populated by TokenClaimsMiddleware
            var staffIdentityNumber = User.Claims
                .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(staffIdentityNumber))
                return Unauthorized("Cannot determine staff identity from headers.");

            var patientData = await roomProfileService.GetControllersHandledByStaffAsync(staffIdentityNumber);
            return Ok(patientData);
        }
        #endregion
    }
}
