using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Signal
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalController : ControllerBase
    {
        #region Attributes
        private readonly IHubContext<SignalHub> hubContext;
        #endregion

        #region Properties
        #endregion

        public SignalController(IHubContext<SignalHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        [HttpPost("publish")]
        public async Task<IActionResult> Publish([FromBody] SignalREnvelope.SignalREnvelope envelope)
        {
            await hubContext.Clients.Group(envelope.Topic)
                .SendAsync(envelope.Method, envelope.Payload);
            return Ok();
        }
    }
}
