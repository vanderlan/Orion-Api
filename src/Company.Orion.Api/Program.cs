using Company.Orion.Api;
using Company.Orion.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

//Must be the first middlewares: every unhandled exception and every empty error status code
//(401/403/404 produced by the framework) is answered as RFC 9457 Problem Details.
app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseRequestLogging();

app.ConfigureApp();

app.RunMigrations();

await app.RunAsync();
