using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Orion.Api.Attributes;
using Orion.Api.AutoMapper;
using Orion.Api.Configuration;
using Orion.Api.Validators;
using Orion.Ioc.Dependencies;

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

        services.AddValidatorsFromAssemblyContaining<CustomerValidator>();

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

        services.AddDomainServices();

        services.ConfigureAutoMapper();

        services.AddHttpContextAccessor();
    }
}
