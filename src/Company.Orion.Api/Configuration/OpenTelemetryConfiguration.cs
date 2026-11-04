using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Company.Orion.Api.Configuration;

public static class OpenTelemetryConfiguration
{
    /// <summary>Standard OTLP environment variable; when set, traces, metrics and logs are exported to it.</summary>
    public const string OtlpEndpointVariable = "OTEL_EXPORTER_OTLP_ENDPOINT";

    /// <summary>
    /// Traces (incoming requests, outgoing HTTP calls), metrics (ASP.NET Core, HttpClient, .NET runtime)
    /// and logs through OpenTelemetry. Nothing is exported unless <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> is
    /// configured (e.g. the Aspire dashboard of docker-compose, an OpenTelemetry Collector, Jaeger, Grafana...).
    /// </summary>
    public static void ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration["OpenTelemetry:ServiceName"] ?? configuration["Serilog:AppName"] ?? "Orion API";
        var serviceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        var openTelemetry = services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName, serviceVersion: serviceVersion, serviceInstanceId: Environment.MachineName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    //Health probes are polled constantly and would drown the real traffic.
                    options.Filter = httpContext => !httpContext.Request.Path.StartsWithSegments(HealthCheckConfiguration.BasePath);
                })
                .AddHttpClientInstrumentation())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation())
            .WithLogging();

        if (!string.IsNullOrWhiteSpace(configuration[OtlpEndpointVariable]))
        {
            //Endpoint, protocol and headers are read from the standard OTEL_EXPORTER_OTLP_* variables.
            openTelemetry.UseOtlpExporter();
        }
    }
}
