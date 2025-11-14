using Application.Interface.IMessagePublisher;
using Application.Interface.IService;
using Domain.IRepository;
using Infrastructure.ExternalService.Security;
using Infrastructure.InfrastructureException;
using Infrastructure.MessageBroker.Publisher;
using Infrastructure.Messaging.Consumer;
using Infrastructure.Persistence.Repository;
using Infrastructure.Service;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                    logger, "Configuring SQL Server database connection");

                // Configure the database connection
                var connectionString = Environment.GetEnvironmentVariable("IAM_DB_CONNECTION");
                if (string.IsNullOrEmpty(connectionString))
                {
                    InfrastructureLoggerBase(
                        logger, "Missing environment variable: IAM_DB_CONNECTION");
                    throw new DatabaseConnectionException(
                        "Failed to configure IAM database.");
                }

                services.AddDbContext<IAMDBContext>(options =>
                    options.UseSqlServer(connectionString));

                // Register repositories + Unit of Work + Mapper

                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IRoleRepository, RoleRepository>();
                services.AddScoped<IAuditLogRepository, AuditLogRepository>();
                services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
                services.AddScoped<IPrivilegeRepository, PrivilegeRepository>();

                services.AddScoped<IUnitOfWork, UnitOfWork>();

                InfrastructureLoggerBase(
                    logger, "Database and repositories configured successfully.");
            }
            catch (Exception ex)
            {
                InfrastructureLoggerBase(
                    logger, "Failed to configure IAM database.", ex);
                throw new DatabaseConnectionException(
                    "Failed to configure IAM database.");
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

                        // Bind IAM consumers to their respective queues
                        cfg.ReceiveEndpoint("iam_update_consumer_queue", e =>
                        {
                            e.ConfigureConsumer<UpdateUserConsumer>(context);
                        });
                    });
                });

                services.AddScoped<IUpdateUserPublisher, UpdateUserPublisher>();
                services.AddScoped<IDeleteUserPublisher, DeleteUserPublisher>();

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
                //InfrastructureLoggerBase(
                //    logger, "Configuring gRPC connection.");

                //var grpcUrl = Environment.GetEnvironmentVariable("GRPC_URL");
                //if (string.IsNullOrEmpty(grpcUrl))
                //{
                //    InfrastructureLoggerBase(
                //        logger, "Missing environment variable: GRPC_URL");
                //    throw new GrpcCommunicationException(
                //        "Failed to configure gRPC client.");
                //}

                // services.AddSingleton(sp =>
                // {
                //     try
                //     {
                //         var channel = GrpcChannel.ForAddress(grpcUrl);
                //         return channel;
                //     }
                //     catch (Exception ex)
                //     {
                //         InfrastructureLoggerBase(
                //         logger, "Failed to initialize gRPC channel.", ex);
                //         throw new GrpcCommunicationException(
                //         "Failed to initialize gRPC channel.");
                //     }
                // });

                // InfrastructureLoggerBase(
                // logger, "gRPC configured successfully.");
            }
            catch (Exception ex)
            {
                InfrastructureLoggerBase(
                    logger, "gRPC configuration failed.", ex);
                throw new GrpcCommunicationException(
                    "Failed to configure gRPC client.");
            }

             //======================
             //4.JWT Token
             //======================
            try
            {
                InfrastructureLoggerBase(
                    logger, "Configuring JWT token");

                var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
                var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
                var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
                var expiryStr = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES");

                if (string.IsNullOrEmpty(secretKey))
                {
                    InfrastructureLoggerBase(
                        logger, "Missing environment variable: JWT_SECRET_KEY");
                    throw new InvalidJWTTokenException(
                        "Failed to configure JWT token service.");
                }
                if (string.IsNullOrEmpty(issuer))
                {
                    InfrastructureLoggerBase(
                        logger, "Missing environment variable: JWT_ISSUER");
                    throw new InvalidJWTTokenException(
                        "Failed to configure JWT token service.");
                }
                if (string.IsNullOrEmpty(audience))
                {
                    InfrastructureLoggerBase(
                        logger, "Missing environment variable: JWT_AUDIENCE");
                    throw new InvalidJWTTokenException(
                        "Failed to configure JWT token service.");
                }
                if (string.IsNullOrEmpty(expiryStr))
                {
                    InfrastructureLoggerBase(
                        logger, "Missing environment variable: JWT_EXPIRY_MINUTES");
                    throw new InvalidJWTTokenException(
                        "Failed to configure JWT token service.");
                }

                var expiryMinutes = int.TryParse(expiryStr, out var exp) ? exp : 60;

                services.AddSingleton<ITokenService>(sp =>
                    new JWTTokenService(secretKey, issuer, audience, expiryMinutes));

                InfrastructureLoggerBase(
                    logger, "JWT token successfully configured.");
            }
            catch (Exception ex)
            {
                InfrastructureLoggerBase(
                    logger, "JWT token configuration failed.", ex);
                throw new InvalidJWTTokenException(
                    "Failed to configure JWT token service.");
            }

             //======================
             //5.Email Service
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
