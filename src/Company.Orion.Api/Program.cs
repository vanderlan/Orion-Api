using Company.Orion.Api.Configuration;
using Company.Orion.Api.Middleware;
using Company.Orion.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureLogging();

builder.Host.UseSerilog();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionMiddleware>();

app.ConfigureApp();

app.RunMigrations();

await app.RunAsync();
