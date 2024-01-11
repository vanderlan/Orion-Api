var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Orion_Api>("orion-api");

builder.Build().Run();