using Company.Orion.Api;
using Company.Orion.Api.Configuration;
using Company.Orion.Api.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<OrionMiddleware>();

app.ConfigureApp();

app.RunMigrations();

await app.RunAsync();