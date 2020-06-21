using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using VBaseProject.Api;
using VBaseProject.Api.AutoMapper.Config;
using VBaseProject.Api.Handler;
using VBaseProject.Service.DependenciesConfig;
using static VBaseProject.Service.Authentication.AuthenticationConfiguration;

namespace VBaseProject
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebHostEnvironment _env;
        public IConfiguration Configuration { get; set; }

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _env = env;
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            ConfigAuthentication(services);

            services.AddControllers();

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            })
            .AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

            services.AddLocalization(options => options.ResourcesPath = @"Resources");

            #region Swagger Config

            //To Disable Swagger in production environment
            //if (!_env.IsProduction())

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "VBaseProject API",
                    Description = "VBase Project - API Description Here!"
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

            #endregion

            #region API Version Config
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });
            #endregion

            DependenciesInjectionConfiguration.Configure(services);
            services.AddAutoMapper(typeof(Startup));
            ConfigureMapper(services);
        }

        private static void ConfigAuthentication(IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(
                            option =>
                            {
                                option.Password.RequireDigit = false;
                                option.Password.RequiredLength = 6;
                                option.Password.RequireNonAlphanumeric = false;
                                option.Password.RequireUppercase = false;
                                option.Password.RequireLowercase = false;
                            }
                        ).AddDefaultTokenProviders();

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
                    ValidAudience = JWT.Audience,
                    ValidIssuer = JWT.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT.SymmetricSecurityKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public void ConfigureMapper(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new InputToDomainProfile());
                mc.AddProfile(new DomainToOutputProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddScoped(_ => { return mapper; });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            var logger = _loggerFactory.CreateLogger<Startup>();

            //ENVIRONMENT
            if (env.IsDevelopment())
            {
                logger.LogInformation("Development environment");
            }
            else
            {
                logger.LogInformation($"Environment: {_env.EnvironmentName}");
                app.UseHsts();
            }
            app.UseVBaseProjectMiddleware();

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
            ;

            #region Globalization configuration

            var defaultCultureInfo = new CultureInfo("pt-BR");
            defaultCultureInfo.NumberFormat.CurrencySymbol = "R$";

            var supportedCultures = new List<CultureInfo>
            {
                defaultCultureInfo,
                new CultureInfo("pt-BR")
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

            #endregion

            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
