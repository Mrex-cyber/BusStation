using System.Globalization;

namespace BusStation.Middlewares
{
    public class RequestKeyChecker
    {
        private readonly RequestDelegate _next;
        private readonly string requestHeaderKey = "key123";

        public RequestKeyChecker(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var keyHeader = context.Request.Headers["RequestKey"];

            if (!string.IsNullOrWhiteSpace(keyHeader))
            {
                if (keyHeader == requestHeaderKey) await _next(context);
                else context.Response.StatusCode = 403;
            }
            else context.Response.StatusCode = 403;

        }
    }
    public static class RequestKeyCheckerExtensions
    {
        public static IApplicationBuilder UseRequestKeyChecker(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestKeyChecker>();
        }
    }
}
