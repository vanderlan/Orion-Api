using System.Diagnostics;
using Company.Orion.Api.ExceptionHandling;
using Microsoft.AspNetCore.Diagnostics;

namespace Company.Orion.Api.Configuration;

public static class ProblemDetailsConfiguration
{
    /// <summary>
    /// Every error produced by the API (domain exceptions, validation, model binding, empty 401/403/404
    /// from the framework) is answered with the same RFC 9457 contract: application/problem+json.
    /// </summary>
    public static void ConfigureProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var request = context.HttpContext.Request;

                context.ProblemDetails.Instance ??= $"{request.Method} {request.Path}";
                context.ProblemDetails.Extensions.TryAdd("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
            };
        });

        services.AddExceptionHandler<DomainExceptionHandler>();

        //The handler already logs every exception with the proper level (warning for 4xx, error for 5xx),
        //so the ExceptionHandlerMiddleware must not log them again as errors.
        services.Configure<ExceptionHandlerOptions>(options => options.SuppressDiagnosticsCallback = _ => true);
    }
}
