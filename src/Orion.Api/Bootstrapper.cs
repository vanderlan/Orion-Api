using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Orion.Api.Attributes;
using Orion.Api.Configuration;
using Orion.Application.Core;
using Orion.Application.Core.Commands.UserCreate;
using Orion.Crosscutting.Ioc.Dependencies;
using Orion.Domain.Core.Authentication;

namespace Orion.Api;

public static class Bootstrapper
{
    public static void ConfigureApp(this IApplicationBuilder app)
    {
        app.UseSwagger();
        
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Orion API");
        });

        app.UseCors(options => options.WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .AllowAnyHeader());

        app.ConfigureGlobalization();

        app.ConfigureHealthCheck();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();

        services.ConfigureAuthentication(configuration);

        services.AddControllers();

        services.AddFluentValidationAutoValidation();

        services.AddValidatorsFromAssemblyContaining<UserCreateRequestValidator>();

        services.ConfigureApiVersioning();

        services.AddMvc(options =>
                        {
                            options.Filters.Add(typeof(CustomValidationAttribute));
                        })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddLocalization(options => options.ResourcesPath = @"Resources");
        services.AddApplicationHealthChecks(configuration);

        services.ConfigureSwagger();

        services.AddDatabaseContext();
        services.AddUnitOfWork();
        services.AddDomainServices();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationAssembly).Assembly));
    }
}
