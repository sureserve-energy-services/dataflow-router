using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.FileProcessing;

namespace Sureserve.Dataflows.Router;

public class DataflowsRouterWorker(ILogger<DataflowsRouterWorker> logger, EnvironmentConfigs config) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (EnvironmentConfig envConfig in config.Environments)
        {
            var fileChecker = new DataFlowsFileChecker(logger, envConfig);
            await fileChecker.CheckForFilesAsync(stoppingToken);
        }
    }
}