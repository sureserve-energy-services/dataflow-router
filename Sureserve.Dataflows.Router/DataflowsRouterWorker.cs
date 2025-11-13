using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.ElectricDataflows;

namespace Sureserve.Dataflows.Router;

public class DataflowsRouterWorker(ILogger<DataflowsRouterWorker> logger, PathConfig config) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var electricFileChecker = new ElectricDataFlowsFileChecker(logger, config);
        await electricFileChecker.CheckForFilesAsync(stoppingToken);
    }
}