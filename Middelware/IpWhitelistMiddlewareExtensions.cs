namespace SteveAPI.Middleware
{
    public static class IpWhitelistMiddlewareExtensions
    {
        public static IApplicationBuilder UseIpWhitelist(this IApplicationBuilder app) =>
            app.UseMiddleware<IpWhitelistMiddleware>();
    }
}
