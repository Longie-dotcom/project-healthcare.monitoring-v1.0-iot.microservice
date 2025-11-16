using Application.Interface.IMessageBrokerPublisher;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Publisher
{
    public class SignalREnvelopePublisher : ISignalREnvelopePublisher
    {
        #region Attributes
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<SignalREnvelope.SignalREnvelope> _logger;
        #endregion

        #region Properties
        #endregion

        public SignalREnvelopePublisher(
            IPublishEndpoint publishEndpoint, ILogger<SignalREnvelope.SignalREnvelope> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        #region Methods
        public async Task PublishAsync(SignalREnvelope.SignalREnvelope envelope)
        {
            try
            {
                _logger.LogInformation(
                    $"Publish an envelop to signal R service: " +
                    $"Topic:{envelope.Topic}-" +
                    $"Method:{envelope.Method}-" +
                    $"Payload:{envelope.Payload}-" +
                    $"Source:{envelope.SourceService}");
                await _publishEndpoint.Publish<SignalREnvelope.SignalREnvelope>(envelope);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when publish an envelop to signal R service: " +
                    $"Topic:{envelope.Topic}-" +
                    $"Method:{envelope.Method}-" +
                    $"Payload:{envelope.Payload}-" +
                    $"Source:{envelope.SourceService}, " +
                    $"{ex.Message}");
            }
        }
        #endregion
    }
}
