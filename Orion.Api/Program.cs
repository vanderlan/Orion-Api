using Orion.Api;
using Orion.Api.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureLogging();

builder.Host.UseSerilog();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.ConfigureApp();

app.Run();
