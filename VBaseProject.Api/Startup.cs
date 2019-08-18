using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;
using VBaseProject.Api;
using VBaseProject.Api.AutoMapper.Config;
using VBaseProject.Api.Handler;
using VBaseProject.Data.Context;
using VBaseProject.Data.UnitOfWork;
using VBaseProject.Service.Implementation;
using VBaseProject.Service.Interfaces;
using static VBaseProject.Service.Authentication.AuthenticationConfiguration;

namespace VBaseProject
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHostingEnvironment _env;

        public Startup(
            IConfiguration configuration,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            ConfigAuthentication(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            })
            .AddJsonOptions(
                j => j.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
             )
            .AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.Configure<DatabaseOptions>(Configuration.GetSection("DatabaseOptions"));

            #region Swagger Config
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "VBaseProject API",
                    Description = "V Base Project - A project reference for building C# and ASP Net Core REST APIs",
                    TermsOfService = "Private",
                });
            });
            #endregion

            //API VERSION CONFIG
            services.AddApiVersioning(o => {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });

            ConfigureDependencyInjection(services);
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT.SymmetricSecurityKey))
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

        private static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWorkEntity, UnitOfWorkEntity>();
            services.AddScoped<IUnitOfWorkDapper, UnitOfWorkDapper>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var logger = _loggerFactory.CreateLogger<Startup>();

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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseCors(options => options.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
            app.UseMvc();

            var builder = new ConfigurationBuilder()

               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
    }
}
