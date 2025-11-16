using MassTransit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Consumer
{
    public class SensorDataConsumer : IConsumer<Dictionary<string, object>>
    {
        public Task Consume(ConsumeContext<Dictionary<string, object>> context)
        {
            try
            {
                var message = context.Message;

                var zeroTierIp = message.TryGetValue("ZeroTierIp", out var ipObj) ? ipObj?.ToString() ?? "Unknown" : "Unknown";
                var temperature = message.TryGetValue("Temperature", out var tempObj) && double.TryParse(tempObj?.ToString(), out var temp) ? temp : 0.0;
                var red = message.TryGetValue("Red", out var redObj) && int.TryParse(redObj?.ToString(), out var redVal) ? redVal : 0;
                var ir = message.TryGetValue("Ir", out var irObj) && int.TryParse(irObj?.ToString(), out var irVal) ? irVal : 0;

                Console.WriteLine($"📩 From {zeroTierIp}: Temp={temperature:F2} RED={red} IR={ir}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error consuming sensor_data: " + ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}

// DTOs
namespace DataCollectionDTO
{
    public class SensorData
    {
        public string ZeroTierIp { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public int Red { get; set; }
        public int Ir { get; set; }
    }
}