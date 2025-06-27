using System.Net;

namespace SteveAPI.Middleware
{
    public class IpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly IPAddress AllowedIp = IPAddress.Parse("187.155.101.200");

        public IpWhitelistMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // Después de UseForwardedHeaders, esta IP es la del cliente real
            var remoteIp = context.Connection.RemoteIpAddress?.MapToIPv4();

            if (remoteIp == null || !remoteIp.Equals(AllowedIp))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: IP not allowed.");
                return;
            }

            await _next(context);
        }
    }
}
