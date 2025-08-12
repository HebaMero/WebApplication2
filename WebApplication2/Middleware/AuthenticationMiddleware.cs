using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        // For demo: a small whitelist of valid tokens.
        // In real apps, validate JWTs or use Identity.
        private static readonly string[] ValidTokens = new[]
        {
            "valid-token-123",   // demo token
            "another-good-token" // demo token
        };

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Allow anonymous access to swagger and possibly health endpoints
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/swagger") || path.StartsWith("/health") || context.Request.Method == HttpMethods.Options)
            {
                await _next(context);
                return;
            }

            // Look for Authorization header: "Bearer <token>"
            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
            {
                await ReturnUnauthorized(context, "Missing Authorization header");
                return;
            }

            var authHeader = authHeaderValues.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                await ReturnUnauthorized(context, "Invalid Authorization header format");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (!IsValidToken(token))
            {
                _logger.LogWarning("Invalid token attempted for request {Method} {Path}", context.Request.Method, context.Request.Path);
                await ReturnUnauthorized(context, "Invalid token");
                return;
            }

            // Optionally set a user identity/principal on context here
            // context.User = ...

            await _next(context);
        }

        private static bool IsValidToken(string token)
        {
            return !string.IsNullOrEmpty(token) && Array.Exists(ValidTokens, t => t.Equals(token, StringComparison.Ordinal));
        }

        private static async Task ReturnUnauthorized(HttpContext context, string reason)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new { error = "Unauthorized", reason });
            await context.Response.WriteAsync(payload);
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseSimpleAuthentication(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
