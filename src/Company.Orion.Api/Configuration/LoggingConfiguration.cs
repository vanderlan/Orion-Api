using Serilog;
using Serilog.Events;

namespace Company.Orion.Api.Configuration;

public static class LoggingConfiguration
{
    /// <summary>
    /// Serilog is the logging pipeline of the host (ILogger&lt;T&gt; everywhere). Events are enriched with
    /// the correlation id, the environment and — automatically by Serilog 4+ — the TraceId/SpanId of the
    /// current OpenTelemetry activity, so console logs can be correlated with traces.
    /// <c>writeToProviders: true</c> also forwards every event to the Microsoft.Extensions.Logging
    /// providers, which is how logs reach the OpenTelemetry exporter.
    /// </summary>
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        //The default providers (Console, Debug...) are removed, otherwise writeToProviders would print every event twice.
        builder.Logging.ClearProviders();

        builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithCorrelationId()
                .Enrich.WithCorrelationIdHeader()
                .Enrich.WithProperty("AppName", builder.Configuration["Serilog:AppName"] ?? "Orion API")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{TraceId} {CorrelationId}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Debug(),
            writeToProviders: true);
    }

    /// <summary>
    /// One structured line per request ("HTTP POST /Auth/Login responded 200 in 12 ms") instead of the
    /// verbose framework logs. Health probes are demoted to Verbose so they do not flood the logs.
    /// </summary>
    public static void UseRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (httpContext, _, exception) =>
            {
                if (exception is not null || httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                    return LogEventLevel.Error;

                return httpContext.Request.Path.StartsWithSegments(HealthCheckConfiguration.BasePath)
                    ? LogEventLevel.Verbose
                    : LogEventLevel.Information;
            };
        });
    }
}
