using Providor.Logging;
using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.FileProcessing;

namespace Sureserve.Dataflows.Router.DependencyInjection;

public static class DataflowsRouterDependencies
{
    public static IServiceCollection AddDataflowsRouterService(this IServiceCollection services, IConfigurationRoot config)
    {
        ILoggingConfig? loggingConfig = GetLoggingConfig(config);
        EnvironmentConfigs environmentConfigs = GetEnvironmentConfigs(config);
        services.AddSingleton(environmentConfigs);
        services.AddSingleton(config);
        services.AddProvidorLogging("DataflowsRouter", loggingConfig ?? new LoggingConfig());
        services.AddTransient<FilesChecker>();
        services.AddTransient<FileProcessor>();
        services.AddHostedService<DataflowsRouterWorker>();
        return services;
    }

    private static EnvironmentConfigs GetEnvironmentConfigs(IConfigurationRoot config)
    {
        EnvironmentConfigs? environmentConfigs = null;

        if (config.GetSection("EnvironmentConfigs").Exists())
        {
            environmentConfigs = config.GetSection("EnvironmentConfigs").Get<EnvironmentConfigs>();
        }

        return environmentConfigs ?? new EnvironmentConfigs();
    }

    private static ILoggingConfig? GetLoggingConfig(IConfigurationRoot config)
    {
        LoggingConfig? loggingConfig = null;

        if (config.GetSection("LoggingConfig").Exists())
        {
            loggingConfig = config.GetSection("LoggingConfig").Get<LoggingConfig>();
        }

        return loggingConfig;
    }
}