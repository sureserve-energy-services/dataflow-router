using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.Interfaces;

namespace Sureserve.Dataflows.Router.ElectricDataflows;

public class ElectricDataFlowsFileChecker(ILogger logger, PathConfig config) : IFileChecker
{
    public async Task CheckForFilesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            PhysicalFileProvider fileWatcher = new($"{config.ElectricDataFlowsIncomingFolderPath}");
            IEnumerable<IFileInfo> changedFiles =
                (PhysicalDirectoryContents)fileWatcher.GetDirectoryContents(string.Empty);
            changedFiles = changedFiles.Where(cf => !cf.IsDirectory);
            int fileProcessCount = 0;

            if (changedFiles.Any())
            {
                logger.LogInformation("Found {changedFilesCount} new electric dataflows.", changedFiles.Count());
            }

            foreach (IFileInfo changedFile in changedFiles)
            {
                await ProcessFile(changedFile);
                fileProcessCount++;
            }

            if (fileProcessCount > 0)
            {
                logger.LogInformation("Processed {fileProcessCount} files", fileProcessCount);
            }
        }
    }
    
    private async Task ProcessFile(IFileInfo changedFile)
    {
        // Placeholder for file processing logic
        logger.LogInformation("Processing file: {fileName}", changedFile.Name);
        await Task.Delay(1000); // Simulate some processing time
        logger.LogInformation("Finished processing file: {fileName}", changedFile.Name);
    }
}