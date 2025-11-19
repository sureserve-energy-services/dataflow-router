using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Providor.Logging;
using Sureserve.Dataflows.Router.Configuration;

namespace Sureserve.Dataflows.Router.FileProcessing;

public class FilesChecker(ILogger<FilesChecker> logger, FileProcessor fileProcessor)
{
    public async Task CheckForFilesAsync(EnvironmentConfig config, CancellationToken cancellationToken)
    {
        logger.LogInformation("Looking for files in: {InputPath} for config {Type}-{Flag}.", config.InputPath, config.EnvironmentType, config.Flag);
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
            await ProcessFile(changedFile, config, cancellationToken);
            fileProcessCount++;
        }

        if (fileProcessCount > 0)
        {
            logger.LogInformation("Processed {fileProcessCount} files", fileProcessCount);
        }
    }
    
    private async Task ProcessFile(IFileInfo changedFile, EnvironmentConfig config, CancellationToken cancellationToken)
    {
        string? fileName = Path.GetFileName(changedFile.PhysicalPath);
        
        using (logger.BeginScope(new Dictionary<string, object>()
                   { { LoggingConstants.FileNameString, fileName ?? "N/A" } }))
        {
            try
            {
                logger.LogInformation("Processing file: {fileName}", changedFile.Name);
                await fileProcessor.ProcessFileAsync(changedFile, config, cancellationToken);
                logger.LogInformation("Finished processing file: {fileName}", changedFile.Name);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error processing file: {fileName} - {exception}", changedFile.Name, exception.ToString());
            }
        }
    }
}