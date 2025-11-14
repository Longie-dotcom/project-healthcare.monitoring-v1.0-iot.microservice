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
    public class PatientStatusesController : ControllerBase
    {
        #region Attributes
        private readonly IPatientStatusService patientStatusService;
        #endregion

        #region Properties
        #endregion

        public PatientStatusesController(IPatientStatusService patientStatusService)
        {
            this.patientStatusService = patientStatusService;
        }

        #region Methods
        [AuthorizePrivilege("ViewPatientStatus")]
        [HttpGet("{patientStatusCode}")]
        public async Task<ActionResult<PatientStatusDTO>> GetByIdAsync(
            string patientStatusCode)
        {
            var patientStatus = await patientStatusService.GetByCodeAsync(patientStatusCode);
            return Ok(patientStatus);
        }

        [AuthorizePrivilege("ViewPatientStatus")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientStatusDTO>>> GetPatientStatuses()
        {
            var patientStatuses = await patientStatusService.GetAllAsync();
            return Ok(patientStatuses);
        }

        [AuthorizePrivilege("CreatePatientStatus")]
        [HttpPost]
        public async Task<ActionResult<PatientStatusDTO>> CreatePatientStatus(
            [FromBody] PatientStatusCreateDTO dto)
        {
            var patientStatus = await patientStatusService.CreateAsync(dto);
            return Ok(patientStatus);
        }

        [AuthorizePrivilege("UpdatePatientStatus")]
        [HttpPut("{patientStatusCode}")]
        public async Task<ActionResult<PatientDTO>> UpdatePatient(
            string patientStatusCode, [FromBody] PatientStatusUpdateDTO dto)
        {
            var updatedPatientStatus = 
                await patientStatusService.UpdateAsync(patientStatusCode, dto);
            return Ok(updatedPatientStatus);
        }

        [AuthorizePrivilege("DeletePatientStatus")]
        [HttpDelete("{patientStatusCode}")]
        public async Task<IActionResult> DeletePatient(
            string patientStatusCode, [FromBody] PatientStatusDeleteDTO dto)
        {
            await patientStatusService.DeleteAsync(patientStatusCode, dto);
            return Ok(new { message = "Delete Patient Status successfully" });
        }
        #endregion
    }
}
