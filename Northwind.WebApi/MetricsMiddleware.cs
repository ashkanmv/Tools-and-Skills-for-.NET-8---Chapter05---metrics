using System.Diagnostics;

namespace Northwind.WebApi;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MetricsService _metricsService;

    public MetricsMiddleware(RequestDelegate next, MetricsService metricsService)
    {
        _next = next;
        _metricsService = metricsService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch timer = Stopwatch.StartNew();

        await _next(context);

        timer.Stop();

        if (context.Request.Path.StartsWithSegments("/api/metrics") == false)
        {
            _metricsService.IncrementRequestCount();
            _metricsService.RecordRequestDuration(timer.ElapsedMilliseconds);
        }

    }
}

public static class MetricsMiddlewareExtensions
{
    public static IApplicationBuilder UseMetricsMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MetricsMiddleware>();
    }
}
