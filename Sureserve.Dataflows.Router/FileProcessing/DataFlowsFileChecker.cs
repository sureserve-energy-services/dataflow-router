using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.Interfaces;

namespace Sureserve.Dataflows.Router.FileProcessing;

public class DataFlowsFileChecker(ILogger logger, IEnvironmentConfig config) : IFileChecker
{
    public async Task CheckForFilesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Looking for files in: {InputPath}.", config.InputPath);
            PhysicalFileProvider fileWatcher = new($"{config.InputPath}");
            IEnumerable<IFileInfo> changedFiles =
                (PhysicalDirectoryContents)fileWatcher.GetDirectoryContents(string.Empty);
            changedFiles = changedFiles.Where(cf => !cf.IsDirectory);
            int fileProcessCount = 0;

            if (changedFiles.Any())
            {
                logger.LogInformation("Found {changedFilesCount} new dataflows.", changedFiles.Count());
            }

            foreach (IFileInfo changedFile in changedFiles)
            {
                await ProcessFile(changedFile, cancellationToken);
                fileProcessCount++;
            }

            if (fileProcessCount > 0)
            {
                logger.LogInformation("Processed {fileProcessCount} files", fileProcessCount);
            }
        }
    }
    
    private async Task ProcessFile(IFileInfo changedFile, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing file: {fileName}", changedFile.Name);
        IFileProcessor fileProcessor = FileProcessorFactory.Create(changedFile.Name, config);
        await fileProcessor.ProcessFileAsync(cancellationToken);
        logger.LogInformation("Finished processing file: {fileName}", changedFile.Name);
    }
}