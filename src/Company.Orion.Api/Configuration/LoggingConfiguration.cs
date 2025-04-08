using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;

namespace Company.Orion.Api.Configuration;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithCorrelationId()
            .Enrich.WithCorrelationIdHeader()
            .WriteTo.Console(outputTemplate: "[{CorrelationId} - {Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Debug()
            .WriteTo.Elasticsearch([new Uri(configuration["ElasticConfiguration:Uri"])], opts =>
            {
                opts.BootstrapMethod = BootstrapMethod.Failure;
                opts.DataStream = new DataStreamName("logs", configuration["Serilog:IndexFormat"]);
                opts.ConfigureChannel = channelOpts => {
                    channelOpts.BufferOptions = new BufferOptions { ExportMaxConcurrency = 10 };
                };
            }, transport =>
            {
                transport.Authentication(new BasicAuthentication("elastic", "change"));
            })
            .Enrich.WithProperty("AppName", configuration["Serilog:AppName"])
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}