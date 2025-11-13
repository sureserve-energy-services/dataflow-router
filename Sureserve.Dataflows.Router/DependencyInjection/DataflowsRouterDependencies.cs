using Providor.Logging;

namespace Sureserve.Dataflows.Router.DependencyInjection;

public static class DataflowsRouterDependencies
{
    public static IServiceCollection AddDataflowsRouterService(this IServiceCollection services, IConfigurationRoot config)
    {
        ILoggingConfig? loggingConfig = GetLoggingConfig(config);
        services.AddSingleton(config);
        services.AddProvidorLogging("MHHS", loggingConfig ?? new LoggingConfig());
        services.AddHostedService<DataflowsRouterWorker>();
        return services;
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