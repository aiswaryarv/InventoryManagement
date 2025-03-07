using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.Threading.Tasks;

public class SerilogRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public SerilogRequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Start stopwatch to log the request duration
        var stopwatch = Stopwatch.StartNew();

        // Log the incoming request
        Log.Information("Incoming request: {Method} {Path} at {Time}",
            context.Request.Method, context.Request.Path, DateTime.UtcNow);

        // Call the next middleware in the pipeline
        await _next(context);

        // Log the outgoing response and the time it took to process the request
        stopwatch.Stop();
        Log.Information("Response: {StatusCode} for {Method} {Path} took {Duration}ms",
            context.Response.StatusCode, context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
    }
}
