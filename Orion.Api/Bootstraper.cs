using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Orion.Api.AutoMapper.Config;
using Orion.Api.Jwt;
using Orion.Api.Middleware;
using Orion.Api.Validators;
using Orion.Ioc.Dependencies;
using System.Globalization;
using System.Text;

namespace Orion.Api
{
    public static class Bootstraper
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors();

            ConfigureAuthentication(services, configuration);

            services.AddControllers();

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<CustomerValidator>();

            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                             .AddDataAnnotationsLocalization();

            services.AddLocalization(options => options.ResourcesPath = @"Resources");
            services.AddHealthChecks();

            ConfigureSwagger(services);
            ConfigureApiVersioning(services);

            services.AddDomainServices();
            
            ConfigureMapper(services);
        }

        private static void ConfigureApiVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            //To Disable Swagger in production environment
            //if (!_env.IsProduction())

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Orion API",
                    Description = "Orion - API Description Here!"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                        Enter 'Bearer' [space] and then your token in the text input below.
                        \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfiguration = configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>();
          
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = jwtConfiguration.Audience,
                    ValidIssuer = jwtConfiguration.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SymmetricSecurityKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static void ConfigureMapper(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new InputToDomainProfile());
                mc.AddProfile(new DomainToOutputProfile());
            });

            services.AddScoped(_ => mappingConfig.CreateMapper());
        }

        public static void ConfigureApp(this IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseMiddleware<OrionMiddleware>();

            //SWAGGER
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //CORS, Origin|Methods|Header|Credentials
            app.UseCors(options => options.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowAnyHeader());

            ConfigureGlobalization(app);

            app.UseHealthChecks("/health-check");

            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private static void ConfigureGlobalization(IApplicationBuilder app)
        {
            var defaultCultureInfo = new CultureInfo("pt-BR")
            {
                NumberFormat =
                {
                    CurrencySymbol = "R$"
                }
            };

            var supportedCultures = new List<CultureInfo>
            {
                defaultCultureInfo,
                new CultureInfo("en-US")
            };

            var globalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            };

            app.UseRequestLocalization(globalizationOptions);

            CultureInfo.DefaultThreadCurrentCulture = defaultCultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCultureInfo;
        }
    }
}
