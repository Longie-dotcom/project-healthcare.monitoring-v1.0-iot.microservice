using MassTransit;
using System.Text.Json;

public class SensorDataConsumer : IConsumer<RawSensorData>
{
    public Task Consume(ConsumeContext<RawSensorData> context)
    {
        try
        {
            var data = JsonSerializer.Deserialize<SensorData>(context.Message.Json);
            if (data != null)
                Console.WriteLine($"📩 From {data.ZeroTierIp}: Temp={data.Temperature:F2} RED={data.Red} IR={data.Ir}");
            else
                Console.WriteLine("⚠️ Empty or invalid JSON");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error consuming message: " + ex.Message);
        }

        return Task.CompletedTask;
    }
}

// DTO for sensor data
public class RawSensorData
{
    public string Json { get; set; } = string.Empty;
}

// Original DTO
public class SensorData
{
    public string ZeroTierIp { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public int Red { get; set; }
    public int Ir { get; set; }
}
