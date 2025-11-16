using Infrastructure;
using Microsoft.Extensions.Logging.Console;
using Signal;
using DotNetEnv;

Env.Load(); // load .env variables

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Logging
// -----------------------------
var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.ClearProviders();
    logging.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "[HH:mm:ss] ";
        options.ColorBehavior = LoggerColorBehavior.Enabled;
    });
    logging.SetMinimumLevel(LogLevel.Information);
});

// -----------------------------
// CORS
// -----------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true);
    });
});

// -----------------------------
// Infrastructure (MassTransit + RabbitMQ)
// -----------------------------
builder.Services.AddInfrastructure(loggerFactory);

// -----------------------------
// SignalR
// -----------------------------
builder.Services.AddSignalR();

// -----------------------------
// Controllers
// -----------------------------
builder.Services.AddControllers();

// -----------------------------
// Swagger
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
// Kestrel Configuration
// -----------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP endpoint for API and SignalR
    options.ListenAnyIP(5100, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });

    // HTTPS endpoint (optional, can disable if not needed)
    options.ListenAnyIP(5101, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

// -----------------------------
// Build
// -----------------------------
var app = builder.Build();

// -----------------------------
// Swagger UI
// -----------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SignalR Central Service API v1");
    c.RoutePrefix = string.Empty; // Swagger at root URL
});

// -----------------------------
// Middlewares
// -----------------------------
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

// -----------------------------
// Map SignalR hubs
// -----------------------------
app.MapHub<SignalHub>("/hubs/signal");

// -----------------------------
// Map controllers
// -----------------------------
app.MapControllers();

// -----------------------------
// Health check
// -----------------------------
app.MapGet("/health", () => Results.Ok("SignalR Central Service running"));

// -----------------------------
// Run
// -----------------------------
app.Run();
