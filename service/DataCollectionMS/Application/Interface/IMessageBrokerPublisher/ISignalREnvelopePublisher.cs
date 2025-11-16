namespace Application.Interface.IMessageBrokerPublisher
{
    public interface ISignalREnvelopePublisher
    {
        Task PublishAsync(SignalREnvelope.SignalREnvelope envelope);
    }
}
