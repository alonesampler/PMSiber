using Microsoft.AspNetCore.Builder;
using PMS.Api.Middlewares;

namespace PMS.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseApiMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>(); // if needed log requests
        app.UseMiddleware<PerformanceMiddleware>(); // if needed metrix

        return app;
    }
}
