using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging.Publisher
{
    public class SensorReadingPublisher
    {
        #region Attributes
        private readonly IChannel _channel;
        #endregion

        #region Properties
        #endregion

        public SensorReadingPublisher(IConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.ExchangeDeclareAsync(
                exchange: "SensorReadingExchange",
                type: ExchangeType.Fanout,
                durable: true
            );
        }

        #region Methods
        public async Task PublishAsync<T>(T @event)
        {
            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: "SensorReadingExchange",
                routingKey: "",
                mandatory: true,
                basicProperties: new BasicProperties(),
                body: body
            );
        }
        #endregion
    }
}
