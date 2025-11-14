namespace APIGateway.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            logger.LogInformation($"{context.Request.Method} {context.Request.Path}");
            await next(context);
            logger.LogInformation($"{context.Response.StatusCode}");
        }
    }
}
