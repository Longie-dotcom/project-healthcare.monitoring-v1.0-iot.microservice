using Infrastructure.Messaging;
using MassTransit;

namespace Infrastructure
{
    public static class MessagingModule
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static void InfrastructureLoggerBase(
            ILogger? logger, string message, Exception? ex = null)
        {
            if (ex == null)
                logger?.LogInformation($"[Infrastructure]: {message}");
            else
                logger?.LogError(ex, $"[Infrastructure]: Error - {message}");
        }

        public static IServiceCollection AddInfrastructure(
                this IServiceCollection services,
                ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new Exception("No logger");

            var logger = loggerFactory.CreateLogger("Infrastructure");

            InfrastructureLoggerBase(logger,
                "Starting Infrastructure configuration");

            //======================
            //RabbitMQ
            //======================
            try
            {
                InfrastructureLoggerBase(
                    logger, "Configuring RabbitMQ connection");

                services.AddMassTransit(x =>
                {
                    // Add all consumers for this service
                    x.AddConsumer<EnvelopConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                        var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER");
                        var rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS");

                        if (
                        string.IsNullOrEmpty(rabbitHost)
                        || string.IsNullOrEmpty(rabbitUser)
                        || string.IsNullOrEmpty(rabbitPassword))
                        {
                            InfrastructureLoggerBase(logger, "Missing RabbitMQ environment variables");
                            throw new MessagingConnectionException("Failed to configure message broker.");
                        }

                        cfg.Host(rabbitHost, "/", h =>
                        {
                            h.Username(rabbitUser);
                            h.Password(rabbitPassword);
                        });

                        cfg.ReceiveEndpoint("signalIR_envelop_consumer", e =>
                        {
                            e.ConfigureConsumer<EnvelopConsumer>(context);
                        });
                    });
                });

                InfrastructureLoggerBase(
                    logger, "RabbitMQ successfully configured.");
            }
            catch (Exception ex)
            {
                InfrastructureLoggerBase(
                    logger, "RabbitMQ configuration failed.", ex);
                throw new MessagingConnectionException(
                    "Failed to configure RabbitMQ infrastructure.");
            }

            return services;
        }
        #endregion
    }
}
