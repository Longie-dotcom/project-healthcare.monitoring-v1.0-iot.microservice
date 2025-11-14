using Application.DTO;
using Application.Interface.IService;
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
        #endregion
    }
}
