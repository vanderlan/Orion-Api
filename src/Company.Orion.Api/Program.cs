using Company.Orion.Api;
using Company.Orion.Api.Configuration;
using Company.Orion.Api.Filters;
using Company.Orion.Api.Middleware;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<AutoValidationActionFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AutoValidationActionFilter>();
});

builder.Configuration.ConfigureLogging();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();


app.UseMiddleware<ExceptionMiddleware>();

app.ConfigureApp();

app.RunMigrations();

await app.RunAsync();
