using Orion.Api;
using Orion.Api.Configuration;
using Orion.Api.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureLogging();

builder.Host.UseSerilog();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<OrionMiddleware>();

app.ConfigureApp();

app.RunMigrations();

app.Run();
