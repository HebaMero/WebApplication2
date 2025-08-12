using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log incoming request
            var method = context.Request.Method;
            var path = context.Request.Path;
            _logger.LogInformation("Incoming request: {Method} {Path}", method, path);

            // Let the rest of pipeline run
            await _next(context);

            // After the response is produced, log status code
            var statusCode = context.Response?.StatusCode;
            _logger.LogInformation("Outgoing response: {Method} {Path} responded {StatusCode}", method, path, statusCode);
        }
    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
