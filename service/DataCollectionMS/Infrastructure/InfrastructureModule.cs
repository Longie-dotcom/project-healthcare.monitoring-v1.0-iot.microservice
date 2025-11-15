using Application.Interface.IGrpc;
using Application.Interface.IService;
using Domain.IRepository;
using Infrastructure.Grpc;
using Infrastructure.InfrastructureException;
using Infrastructure.Messaging.Consumer;
using Infrastructure.Persistence.Configuration;
using Infrastructure.Persistence.Repository;
using Infrastructure.Service;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure
{
    public static class InfrastructureModule
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

            // ======================
            // 1. Database
            // ======================
            try
            {
                InfrastructureLoggerBase(
                    logger, "Configuring MongoDB connection");

                var mongoConnection = Environment.GetEnvironmentVariable("DATACOLLECTION_DB_CONNECTION");

                if (string.IsNullOrEmpty(mongoConnection))
                {
                    InfrastructureLoggerBase(
                        logger, "Missing environment variable: DATACOLLECTION_DB_CONNECTION");
                    throw new DatabaseConnectionException(
                        "Failed to configure MongoDB connection.");
                }

                services.AddSingleton(sp => new DataCollectionDBContext(
                    mongoConnection,
                    "DataCollectionDB"));

                // Register repositories (MongoDB-based)
                services.AddScoped<IRoomProfileRepository, RoomProfileRepository>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();

                InfrastructureLoggerBase(
                    logger, "MongoDB and document repositories configured successfully.");
            }
            catch (Exception ex)
            {
                InfrastructureLoggerBase(
                    logger, "Failed to configure MongoDB connection.", ex);
                throw new DatabaseConnectionException(
                    "Failed to configure MongoDB connection.");
            }

            //======================
            //2.RabbitMQ
            //======================
            try
            {
                InfrastructureLoggerBase(
                    logger, "Configuring RabbitMQ connection");

                services.AddMassTransit(x =>
                {
                    // Add all consumers for this service
                    x.AddConsumer<UpdateUserConsumer>();
                    x.AddConsumer<CreateDeviceProfileConsumer>();
                    x.AddConsumer<UpdateDeviceProfileConsumer>();
                    x.AddConsumer<RemoveDeviceProfileConsumer>();
                    x.AddConsumer<PatientStaffAssignmentConsumer>();
                    x.AddConsumer<CreateRoomProfile>();
                    x.AddConsumer<SensorDataConsumer>();

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

                        cfg.ReceiveEndpoint("data_collection_update_user_consumer_queue", e =>
                        {
                            e.ConfigureConsumer<UpdateUserConsumer>(context);
                        });

                        cfg.ReceiveEndpoint("data_collection_create_profile_consumer_queue", e =>
                        {
                            e.ConfigureConsumer<CreateDeviceProfileConsumer>(context);
                        });

                        cfg.ReceiveEndpoint("data_collection_update_profile_consumer_queue", e =>
                        {
                            e.ConfigureConsumer<UpdateDeviceProfileConsumer>(context);
                        });

                        cfg.ReceiveEndpoint("data_collection_remove_profile_consumer_queue", e =>
                        {
                            e.ConfigureConsumer<RemoveDeviceProfileConsumer>(context);
                        });

                        cfg.ReceiveEndpoint("data_collection_staff_assignment_consumer_queue", e =>
                        {
                            e.ConfigureConsumer<PatientStaffAssignmentConsumer>(context);
                        });

                        cfg.ReceiveEndpoint("data_collection_create_room_profile_consumer_queue", e =>
                        {
                            e.ConfigureConsumer<CreateRoomProfile>(context);
                        });

                        cfg.ReceiveEndpoint("sensor_data", e =>
                        {
                            e.ConfigureConsumer<SensorDataConsumer>(context);
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

            //======================
            //3.gRPC Clients
            //======================
            try
            {
                InfrastructureLoggerBase(
                    logger, "Configuring gRPC connection.");

                services.AddScoped<IGrpcClient, GrpcClient>();

                InfrastructureLoggerBase(
                logger, "gRPC configured successfully.");
            }
            catch (Exception ex)
            {
                InfrastructureLoggerBase(
                    logger, "gRPC configuration failed.", ex);
                throw new GrpcCommunicationException(
                    "Failed to configure gRPC client.");
            }

            //======================
            //4.Email Service
            //======================
            try
            {
                InfrastructureLoggerBase(logger, "Configuring Email Service");

                var fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM");
                var displayName = Environment.GetEnvironmentVariable("EMAIL_DISPLAY_NAME");
                var smtpHost = Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST");
                var smtpPortStr = Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT");
                var username = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
                var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
                var enableSslStr = Environment.GetEnvironmentVariable("EMAIL_ENABLE_SSL");

                if (string.IsNullOrEmpty(fromEmail) ||
                    string.IsNullOrEmpty(displayName) ||
                    string.IsNullOrEmpty(smtpHost) ||
                    string.IsNullOrEmpty(smtpPortStr) ||
                    string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(enableSslStr))
                {
                    InfrastructureLoggerBase(logger,
                        "Missing environment variable for Email Service");
                    throw new Exception("Failed to configure Email Service");
                }

                if (!int.TryParse(smtpPortStr, out var smtpPort))
                    smtpPort = 587; // default SMTP port

                if (!bool.TryParse(enableSslStr, out var enableSsl))
                    enableSsl = true;

                services.AddSingleton<IEmailService>(sp =>
                    new EmailService(
                        fromEmail,
                        displayName,
                        smtpHost,
                        smtpPort,
                        username,
                        password,
                        enableSsl));

                InfrastructureLoggerBase(logger, "Email Service successfully configured");
            }
            catch (Exception ex)
            {
                InfrastructureLoggerBase(logger, "Email Service configuration failed", ex);
                throw;
            }

            return services;
        }
        #endregion
    }
}
