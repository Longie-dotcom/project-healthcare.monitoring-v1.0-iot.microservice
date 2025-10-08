using Application;
using Application.Service;
using Domain.Aggregate;
using DotNetEnv;
using Infrastructure;
using Infrastructure.Messaging.Consumer;

Env.Load(); // load environment variables from .env

var builder = WebApplication.CreateBuilder(args);

// 1. Add infrastructure (MongoDB, RabbitMQ, etc.)
builder.Services.AddInfrastructure();

// 2. Add application services
builder.Services.AddApplication();

var app = builder.Build();

// 4. Resolve consumer and application service
var consumer = app.Services.GetRequiredService<SensorReadingConsumer<SensorReading>>();
var readingService = app.Services.GetRequiredService<SensorReadingService>();
consumer.StartConsuming(async reading =>
{
    await readingService.ProcessReadingAsync(
        reading
    );
});

// 6. Optional: map HTTP endpoints for monitoring
app.MapGet("/", () => "Sensor Reading Service Running");

app.Run();
