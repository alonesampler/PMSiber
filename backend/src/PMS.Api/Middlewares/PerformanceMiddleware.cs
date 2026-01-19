using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PMS.Api.Middlewares;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;
    private const long SlowRequestThreshold = 1000; // 1 секунда

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > SlowRequestThreshold)
        {
            _logger.LogWarning(
                "Медленный запрос {Method} {Path} выполнен за {Elapsed}ms",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
        }
    }
}