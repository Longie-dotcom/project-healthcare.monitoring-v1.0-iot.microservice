using API.Models;
using System.Text.Json;

namespace API.Middleware
{
    public class SuccessResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public SuccessResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip gRPC calls
            if (context.Request.ContentType?.StartsWith("application/grpc") == true)
            {
                await _next(context);
                return;
            }

            // Skip SignalR hubs
            if (context.Request.Path.StartsWithSegments("/hubs"))
            {
                await _next(context);
                return;
            }

            var originalBody = context.Response.Body;
            var tempStream = new MemoryStream();
            context.Response.Body = tempStream;

            try
            {
                await _next(context);

                tempStream.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(tempStream).ReadToEndAsync();

                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300 &&
                    !string.IsNullOrWhiteSpace(responseText) &&
                    !responseText.Contains("\"success\":", StringComparison.OrdinalIgnoreCase))
                {
                    object? payload = null;

                    if (IsJson(responseText))
                    {
                        payload = JsonSerializer.Deserialize<object>(responseText,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    else
                    {
                        payload = responseText.Trim('"');
                    }

                    var wrapped = new SuccessResponse
                    {
                        Success = true,
                        Payload = payload,
                        Timestamp = DateTime.UtcNow
                    };

                    var json = JsonSerializer.Serialize(wrapped, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    context.Response.ContentType = "application/json";
                    context.Response.Body = originalBody;
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    tempStream.Seek(0, SeekOrigin.Begin);
                    await tempStream.CopyToAsync(originalBody);
                }
            }
            finally
            {
                context.Response.Body = originalBody;
                tempStream.Dispose();
            }
        }

        private bool IsJson(string input)
        {
            input = input.Trim();
            return (input.StartsWith("{") && input.EndsWith("}")) ||
                   (input.StartsWith("[") && input.EndsWith("]"));
        }
    }
}
