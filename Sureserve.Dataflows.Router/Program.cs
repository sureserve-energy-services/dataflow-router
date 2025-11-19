using Sureserve.Dataflows.Router.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
Console.WriteLine(builder.Environment.EnvironmentName);
IConfigurationRoot config = builder.Configuration;
builder.Services.AddDataflowsRouterService(config);

var host = builder.Build();
host.Run();