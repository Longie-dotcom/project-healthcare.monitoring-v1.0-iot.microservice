using System.Security.Claims;

namespace APIGateway.Middleware
{
    public class TokenForwardingMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenForwardingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var fullName = user.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
                var role = user.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

                // Extract privilege names
                var privileges = user.Claims.FirstOrDefault(c => c.Type == "Privileges")?.Value;

                if (!string.IsNullOrEmpty(userId))
                    context.Request.Headers["X-User-Id"] = userId;
                if (!string.IsNullOrEmpty(email))
                    context.Request.Headers["X-User-Email"] = email;
                if (!string.IsNullOrEmpty(fullName))
                    context.Request.Headers["X-User-FullName"] = fullName;
                if (!string.IsNullOrEmpty(role))
                    context.Request.Headers["X-User-Role"] = role;
                if (!string.IsNullOrEmpty(privileges))
                    context.Request.Headers["X-User-Privileges"] = privileges;
            }

            await _next(context);
        }
    }
}
