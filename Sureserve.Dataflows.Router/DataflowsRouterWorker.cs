using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.FileProcessing;

namespace Sureserve.Dataflows.Router;

public class DataflowsRouterWorker(FilesChecker fileChecker, EnvironmentConfigs config) : BackgroundService
{
    private const int MillisecondsDelay = 15000;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (EnvironmentConfig envConfig in config.Environments)
            {
                await fileChecker.CheckForFilesAsync(envConfig, stoppingToken);
            }
            
            await Task.Delay(MillisecondsDelay, stoppingToken);
        }
    }
}