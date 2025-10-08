using RabbitMQ.Client;
using System.Diagnostics;

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
                _connection = _factory.CreateConnectionAsync().Result;
            }
            return _connection;
        }
        #endregion
    }
}
