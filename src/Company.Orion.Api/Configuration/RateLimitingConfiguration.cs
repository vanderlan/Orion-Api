using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Company.Orion.Api.Configuration;

/// <summary>
/// Section of appsettings that contains the rate limiting configuration.
/// </summary>
public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public PolicyOptions Auth { get; set; } = new();

    public sealed class PolicyOptions
    {
        /// <summary>Maximum number of requests per client (IP address) in each window.</summary>
        public int PermitLimit { get; set; } = 10;

        public int WindowSeconds { get; set; } = 60;
    }
}

public static class RateLimitingConfiguration
{
    /// <summary>Policy applied to the authentication endpoints (credential stuffing / brute force protection).</summary>
    public const string AuthPolicy = "auth";

    public static void ConfigureRateLimiting(this IServiceCollection services)
    {
        services.AddOptions<RateLimitingOptions>()
            .BindConfiguration(RateLimitingOptions.SectionName)
            .Validate(options => options.Auth.PermitLimit > 0 && options.Auth.WindowSeconds > 0,
                "RateLimiting:Auth requires PermitLimit and WindowSeconds greater than zero.")
            .ValidateOnStart();

        services.AddRateLimiter(options =>
        {
            //The 429 body is written by UseStatusCodePages as a ProblemDetails, like every other error.
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(AuthPolicy, httpContext =>
            {
                var settings = httpContext.RequestServices.GetRequiredService<IOptions<RateLimitingOptions>>().Value.Auth;
                var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = settings.PermitLimit,
                    Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                    QueueLimit = 0
                });
            });
        });
    }
}
