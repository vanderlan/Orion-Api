namespace Company.Orion.Api.Configuration;

public static class CorsConfiguration
{
    public const string AllowedOriginsPath = "Cors:AllowedOrigins";

    /// <summary>
    /// Default CORS policy bound from configuration (<c>Cors:AllowedOrigins</c>). An empty list or "*"
    /// allows any origin (fine for development); production settings must list the real front-end origins.
    /// </summary>
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection(AllowedOriginsPath).Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (allowedOrigins.Length == 0 || allowedOrigins.Contains("*"))
                {
                    policy.AllowAnyOrigin();
                }
                else
                {
                    policy.WithOrigins(allowedOrigins);
                }

                policy.AllowAnyHeader().AllowAnyMethod();
            });
        });
    }
}
