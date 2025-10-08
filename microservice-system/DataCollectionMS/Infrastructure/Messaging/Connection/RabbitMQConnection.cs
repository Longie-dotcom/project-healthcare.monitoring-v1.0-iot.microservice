using RabbitMQ.Client;

namespace Infrastructure.Messaging.Connection
{
    public class RabbitMQConnection
    {
        #region Attributes
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        #endregion

        #region Properties
        #endregion

        public RabbitMQConnection(
            string hostName,
            string? userName = null,
            string? password = null)
        {
            _factory = new ConnectionFactory()
            {
                HostName = hostName
            };

            if (!string.IsNullOrEmpty(userName))
            {
                _factory.UserName = userName;
            }

            if (!string.IsNullOrEmpty(password))
            {
                _factory.Password = password;
            }
        }

        #region Methods
        public IConnection GetConnection()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                int retries = 10;
                while (retries-- > 0)
                {
                    try
                    {
                        Console.WriteLine($"[RabbitMQ] Attempting to connect to {_factory.HostName}...");
                        _connection = _factory.CreateConnectionAsync().Result;
                        Console.WriteLine("[RabbitMQ] Connected successfully.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[RabbitMQ] Connection failed ({ex.Message}). Retrying in 5s... ({retries} left)");
                        Task.Delay(5000).Wait();
                    }
                }

                if (_connection == null || !_connection.IsOpen)
                {
                    throw new Exception("Failed to connect to RabbitMQ after several attempts.");
                }
            }
            return _connection;
        }
        #endregion
    }
}
