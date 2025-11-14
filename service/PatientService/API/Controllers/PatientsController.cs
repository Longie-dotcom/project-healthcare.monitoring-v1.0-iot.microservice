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
    public class PatientsController : ControllerBase
    {
        #region Attributes
        private readonly IPatientService patientService;
        #endregion

        #region Properties
        #endregion

        public PatientsController(IPatientService patientService)
        {
            this.patientService = patientService;
        }

        #region Methods
        [AuthorizePrivilege("ViewPatient")]
        [HttpGet("{patientId:guid}")]
        public async Task<ActionResult<PatientDTO>> GetByIdAsync(
            Guid patientId)
        {
            var patient = await patientService.GetByIdAsync(patientId);
            return Ok(patient);
        }

        [AuthorizePrivilege("ViewPatient")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients(
            [FromQuery] string sortBy,
            [FromQuery] QueryPatientDTO dto)
        {
            var patients = await patientService.GetAllAsync(dto, sortBy);
            return Ok(patients);
        }

        [AuthorizePrivilege("CreatePatient")]
        [HttpPost]
        public async Task<ActionResult<PatientDTO>> CreatePatient(
            [FromBody] PatientCreateDTO dto)
        {
            var patient = await patientService.CreateAsync(dto);
            return Ok(patient);
        }

        [AuthorizePrivilege("UpdatePatient")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PatientDTO>> UpdatePatient(
            Guid id, [FromBody] PatientUpdateDTO dto)
        {
            var updatedPatient = await patientService.UpdateAsync(id, dto);
            return Ok(updatedPatient);
        }

        [AuthorizePrivilege("DeletePatient")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePatient(
            Guid id, [FromBody] PatientDeleteDTO dto)
        {
            await patientService.DeleteAsync(id, dto);
            return Ok(new { message = "Delete patient successfully" });
        }

        [AuthorizePrivilege("AssignPatientBed")]
        [HttpPost("{patientId:guid}/assign-bed")]
        public async Task<IActionResult> AssignBed(
            Guid patientId, [FromBody] AssignBedDTO dto)
        {
            await patientService.AssignBedAsync(patientId, dto);
            return Ok(new { message = "Bed assigned successfully." });
        }

        [AuthorizePrivilege("ReleasePatientBed")]
        [HttpPost("{patientId:guid}/release-bed")]
        public async Task<IActionResult> ReleaseBed(
            Guid patientId, [FromBody] ReleaseBedDTO dto)
        {
            await patientService.ReleaseBedAsync(patientId, dto);
            return Ok(new { message = "Bed released successfully." });
        }

        [AuthorizePrivilege("AssignPatientStaff")]
        [HttpPost("{patientId:guid}/assign-staff")]
        public async Task<IActionResult> AssignStaff(
            Guid patientId, [FromBody] AssignStaffDTO dto)
        {
            await patientService.AssignStaffAsync(patientId, dto);
            return Ok(new { message = "Staff assigned successfully." });
        }

        [AuthorizePrivilege("UnassignPatientStaff")]
        [HttpPost("{patientId:guid}/unassign-staff")]
        public async Task<IActionResult> UnassignStaff(
            Guid patientId, [FromBody] UnassignStaffDTO dto)
        {
            await patientService.UnassignStaffAsync(patientId, dto);
            return Ok(new { message = "Staff unassigned successfully." });
        }
        #endregion
    }
}
