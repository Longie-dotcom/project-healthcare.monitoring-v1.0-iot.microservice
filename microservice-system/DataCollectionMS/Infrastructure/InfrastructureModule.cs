using Domain.Aggregate;
using Domain.IRepository;
using Infrastructure.Messaging.Connection;
using Infrastructure.Messaging.Consumer;
using Infrastructure.Messaging.Publisher;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Infrastructure
{
    public static class InfrastructureModule
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // ======================
            // 1. MongoDB
            // ======================
            var mongoConn = Environment.GetEnvironmentVariable("MONGO_CONNECTION")
                            ?? throw new Exception("MONGO_CONNECTION env variable is missing");
            var mongoDbName = Environment.GetEnvironmentVariable("MONGO_DB_NAME")
                              ?? throw new Exception("MONGO_DB_NAME env variable is missing");

            services.AddSingleton(sp => new MongoDBContext(new MongoSettings()
            {
                ConnectionString = mongoConn,
                DatabaseName = mongoDbName,
            }));

            // ASSIGN REPOSITORY
            services.AddSingleton<ISensorReadingRepository>(sp =>
            {
                var mongoContext = sp.GetRequiredService<MongoDBContext>();
                return new Infrastructure.Persistence.Repository.SensorReadingRepository(
                    mongoContext.Database);
            });

            // ======================
            // 2. RabbitMQ
            // ======================
            var rabbitHost = Environment.GetEnvironmentVariable("RABBIT_HOST") ?? "localhost";
            var rabbitUser = Environment.GetEnvironmentVariable("RABBIT_USER") ?? "guest";
            var rabbitPassword = Environment.GetEnvironmentVariable("RABBIT_PASSWORD") ?? "guest";

            services.AddSingleton<RabbitMQConnection>(sp =>
                new RabbitMQConnection(rabbitHost, rabbitUser, rabbitPassword));

            services.AddSingleton<IConnection>(sp =>
            {
                var rabbitConn = sp.GetRequiredService<RabbitMQConnection>();
                return rabbitConn.GetConnection();
            });

            services.AddSingleton<SensorReadingPublisher>(sp =>
            {
                var connection = sp.GetRequiredService<IConnection>();
                return new SensorReadingPublisher(connection);
            });

            services.AddSingleton<SensorReadingConsumer<SensorReading>>(sp =>
            {
                var connection = sp.GetRequiredService<IConnection>();
                return new SensorReadingConsumer<SensorReading>(connection, "SensorQueue");
            });

            // ======================
            // 3. gRPC Clients
            // ======================
            var grpcUrl = Environment.GetEnvironmentVariable("GRPC_URL")
                          ?? throw new Exception("GRPC_URL env variable is missing");

            //services.AddSingleton(sp =>
            //{
            //    var channel = GrpcChannel.ForAddress(grpcUrl);
            //    return channel;
            //});

            return services;
        }
        #endregion
    }
}
