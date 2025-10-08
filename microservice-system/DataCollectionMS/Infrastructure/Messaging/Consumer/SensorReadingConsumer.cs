using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging.Consumer
{
    public class SensorReadingConsumer<T>
    {
        #region Attributes
        private readonly IChannel _channel;
        private readonly string _queueName;
        #endregion

        #region Properties
        #endregion

        public SensorReadingConsumer(IConnection connection, string queueName)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName));
            }

            _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
            _queueName = queueName;

            _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
        }

        #region Methods
        public void StartConsuming(Func<T, Task> handleMessageAsync)
        {
            if (handleMessageAsync == null) throw new ArgumentNullException(nameof(handleMessageAsync));

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var obj = JsonSerializer.Deserialize<T>(message);

                    if (obj != null)
                    {
                        await handleMessageAsync(obj);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                }
            };

            _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer
            );
        }
        #endregion
    }
}
