using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Orion.Api.Attributes;
using Orion.Api.AutoMapper;
using Orion.Api.Configuration;
using Orion.Api.Middleware;
using Orion.Api.Validators;
using Orion.Ioc.Dependencies;

namespace Orion.Api
{
    public static class Bootstraper
    {
        public static void ConfigureApp(this IApplicationBuilder app)
        {
            app.UseMiddleware<OrionMiddleware>();

            //SWAGGER
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orion API");
            });

            //CORS, Origin|Methods|Header|Credentials
            app.UseCors(options => options.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowAnyHeader());

            app.ConfigureGlobalization();

            app.UseHealthChecks("/health-check");

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
            services.AddHealthChecks();

            services.ConfigureSwagger();
            services.ConfigureApiVersioning();

            services.AddDomainServices();

            services.ConfigureAutoMapper();
        }
    }
}
