using System.Text.Json.Serialization;
using Company.Orion.Api.Configuration;
using Company.Orion.Api.Filters;
using Company.Orion.Application.Core;
using Company.Orion.Application.Core.UseCases.Users.Commands.Create;
using Company.Orion.Crosscutting.Ioc.Dependencies;
using Company.Orion.Domain.Core.Authentication;
using FluentValidation;

namespace Company.Orion.Api;

public static class Bootstrapper
{
    public static void ConfigureApp(this WebApplication app)
    {
        app.UseOpenApiDocumentation();

        app.UseCors();

        app.ConfigureGlobalization();

        app.ConfigureHealthCheck();

        app.UseRouting();

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureCors(configuration);

        services.ConfigureAuthentication();

        services.ConfigureRateLimiting();

        services.ConfigureProblemDetails();

        services.AddScoped<AutoValidationActionFilter>();
        services.AddControllers(options =>
            {
                options.Filters.Add<AutoValidationActionFilter>();
            })
            .AddJsonOptions(options =>
            {
                //Enums travel as their names ("Admin") instead of numbers; numbers are still accepted.
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddValidatorsFromAssemblyContaining<UserCreateRequestValidator>();

        services.ConfigureApiVersioning();

        services.AddLocalization(options => options.ResourcesPath = @"Resources");
        services.AddApplicationHealthChecks(configuration);
        services.ConfigureOpenTelemetry(configuration);

        services.ConfigureOpenApi();

        services.AddDatabaseContext(configuration);
        services.AddUnitOfWork();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationAssembly).Assembly));
    }
}
