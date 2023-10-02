using Orion.Api.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Bootstraper.ConfigureLogging(builder.Configuration);

builder.Host.UseSerilog();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.ConfigureApp();

app.Run();
