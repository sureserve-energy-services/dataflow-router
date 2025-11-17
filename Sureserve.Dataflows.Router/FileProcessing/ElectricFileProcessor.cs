using Microsoft.Extensions.FileProviders;
using Providor.Logging;
using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.FileParsers;
using Sureserve.Dataflows.Router.Interfaces;

namespace Sureserve.Dataflows.Router.FileProcessing;

public class ElectricFileProcessor(ILogger logger, IEnvironmentConfig environmentConfig) : IFileProcessor
{
    public async Task ProcessFileAsync(IFileInfo fileInfo, CancellationToken cancellationToken)
    {
        using (logger.BeginScope(new Dictionary<string, object>() { { LoggingConstants.FileNameString, fileInfo.Name } }))
        {
            if (fileInfo != null)
            {
                logger.LogInformation("Processing new file: {changedFileName}", fileInfo.Name);

                try
                {
                    string? fileName = Path.GetFileName(fileInfo.PhysicalPath);
                    if (fileInfo.PhysicalPath != null)
                    {
                        using StreamReader reader = File.OpenText(fileInfo.PhysicalPath);
                        string flag = ElectricFileParser.GetFlagFromFileHeader(await reader.ReadToEndAsync());
                    }
                }
                catch (Exception exception)
                {
                    logger.LogError("Error occurred while processing new file: {changedFileName}. {exceptionMessage}",
                        fileInfo.Name, exception.Message);
                }
            }
        }
    }
}