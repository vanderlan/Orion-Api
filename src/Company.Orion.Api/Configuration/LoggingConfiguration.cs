using Serilog;
using Serilog.Sinks.Elasticsearch;

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
            .WriteTo.Elasticsearch(ConfigureElasticSink(configuration))
            .Enrich.WithProperty("AppName", configuration["Serilog:AppName"])
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration)
    {
        return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
        {
            AutoRegisterTemplate = true,
            IndexFormat = configuration["Serilog:IndexFormat"]
        };
    }
}
