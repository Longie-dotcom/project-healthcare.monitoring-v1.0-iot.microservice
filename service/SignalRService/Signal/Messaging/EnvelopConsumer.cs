using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Signal;

namespace Infrastructure.Messaging
{
    public class EnvelopConsumer : IConsumer<SignalREnvelope.SignalREnvelope>
    {
        #region Attributes
        private readonly IHubContext<SignalHub> hubContext;
        #endregion

        #region Properties
        #endregion

        public EnvelopConsumer(IHubContext<SignalHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        #region Methods
        public async Task Consume(ConsumeContext<SignalREnvelope.SignalREnvelope> envelope)
        {
            var message = envelope.Message;

            await hubContext.Clients.Group(message.Topic)
                .SendAsync(message.Method, message.Payload);
        }
        #endregion
    }
}
