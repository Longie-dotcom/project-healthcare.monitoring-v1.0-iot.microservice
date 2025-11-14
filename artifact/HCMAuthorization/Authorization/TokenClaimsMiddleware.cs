using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HCM.Authorization
{
    public class TokenClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var claims = new List<Claim>();

            void AddHeaderClaim(string headerName, string claimType)
            {
                if (context.Request.Headers.TryGetValue(headerName, out var value) && !string.IsNullOrEmpty(value))
                    claims.Add(new Claim(claimType, value!));
            }

            // Map headers back to claims
            AddHeaderClaim("X-User-Id", ClaimTypes.NameIdentifier);
            AddHeaderClaim("X-User-Email", ClaimTypes.Email);
            AddHeaderClaim("X-User-FullName", "FullName");
            AddHeaderClaim("X-User-Role", ClaimTypes.Role);

            // Handle privileges (comma-separated list)
            if (context.Request.Headers.TryGetValue("X-User-Privileges", out var privilegesHeader))
            {
                var privileges = privilegesHeader.ToString()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var privilege in privileges)
                    claims.Add(new Claim("Privileges", privilege));
            }

            // Build identity if any claim exists
            if (claims.Count > 0)
            {
                var identity = new ClaimsIdentity(claims, "HeaderAuth");
                context.User = new ClaimsPrincipal(identity);
            }

            await _next(context);
        }
    }

}
