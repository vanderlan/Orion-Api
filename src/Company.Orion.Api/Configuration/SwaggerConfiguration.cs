using Microsoft.OpenApi;

namespace Company.Orion.Api.Configuration;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Orion API",
                Description = "Orion API - A simple project template for creating a .NET API (v10.0)"
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

            //c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {
            //            Scheme = "oauth2",
            //            Name = "Bearer",
            //            In = ParameterLocation.Header,
            //            BearerFormat = "JWT",
            //            Type = SecuritySchemeType.ApiKey,
            //            Description = "JWT Authorization header using the Bear",

            //        },
            //        new List<string>()
            //    }
            //});
        });
    }
}
