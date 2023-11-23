var builder = DistributedApplication.CreateBuilder(args);

//builder.AddProject<Projects.DeadDrop_Web>("deaddrop.web");

builder.AddProject<Projects.DeadDrop_Api>("deaddrop.api");

//builder.AddProject<Projects.DeadDrop_Web>("deaddrop.web");

builder.Build().Run();
