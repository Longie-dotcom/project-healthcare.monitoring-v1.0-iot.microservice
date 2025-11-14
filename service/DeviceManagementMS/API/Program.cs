using API.Middleware;
using Application;
using Application.GrpcService;
using DotNetEnv;
using HCM.Authorization;
using Infrastructure;
using Infrastructure.Persistence.Configuration;
using Infrastructure.Persistence.Seed;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;

Env.Load();

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
// Core setup
// -----------------------------
builder.Services.AddGrpc();
builder.Services.AddInfrastructure(loggerFactory);
builder.Services.AddApplication();
builder.Services.AddControllers();

// -----------------------------
// Swagger
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
// Kestrel
// -----------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    // gRPC endpoint — requires HTTP/2
    options.ListenAnyIP(6301, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });

    // REST endpoint — uses standard HTTP/1.1
    options.ListenAnyIP(6300, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });
});

// -----------------------------
// Privilege Authorization
// -----------------------------
builder.Services.AddPrivilegeAuthorization();

var app = builder.Build();

// -----------------------------
// DB migration & seeding
// -----------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DeviceManagementDBContext>();

    var retries = 5;
    for (int i = 0; i < retries; i++)
    {
        try
        {
            await db.Database.MigrateAsync();
            await IAMDBSeed.Seed(db);
            Console.WriteLine("Database migrated and seeded successfully.");
            break;
        }
        catch (SqlException)
        {
            Console.WriteLine($"Database not ready, retrying in 5s... ({i + 1}/{retries})");
            await Task.Delay(5000);
            if (i == retries - 1) throw;
        }
    }
}

// -----------------------------
// Swagger UI
// -----------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IAM Service API v1");
    c.RoutePrefix = string.Empty;
});

// -----------------------------
// Map Grpc
// -----------------------------
app.MapGrpcService<EdgeDeviceGrpcService>();

// -----------------------------
// Middlewares
// -----------------------------
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<SuccessResponseMiddleware>();
app.UseMiddleware<TokenClaimsMiddleware>();

app.UseRouting();
app.UseAuthorization();

// -----------------------------
// Controllers & health endpoint
// -----------------------------
app.MapControllers();
app.MapGet("/health", () => Results.Ok("Staff Service running"));

// -----------------------------
// Run
// -----------------------------
app.Run();
