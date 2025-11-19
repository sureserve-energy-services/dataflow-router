using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.FileProviders;
using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.FileParsers;

namespace Sureserve.Dataflows.Router.FileProcessing;

public class FileProcessor(ILogger<FileProcessor> logger)
{
    public async Task ProcessFileAsync(IFileInfo fileInfo, EnvironmentConfig environmentConfig, CancellationToken cancellationToken)
    {
        if (fileInfo != null)
        {
            if (fileInfo.PhysicalPath != null)
            {
                string extension = Path.GetExtension(fileInfo.Name)?.TrimStart('.') ?? "";
                
                if (environmentConfig.FileExtensions.Select(fe => fe.ToLower()).Contains(extension.ToLower()))
                {
                    string flag;

                    using (StreamReader reader = File.OpenText(fileInfo.PhysicalPath))
                    {
                        flag = ElectricFileParser.GetFlagFromFileHeader(await reader.ReadToEndAsync(cancellationToken));
                    }

                    if (flag == environmentConfig.Flag)
                    {
                        await HandleFileByFlag(fileInfo, environmentConfig, cancellationToken);
                    }
                }
            }
        }
    }

    private async Task HandleFileByFlag(IFileInfo fileInfo, EnvironmentConfig environmentConfig, CancellationToken cancellationToken)
    {
        if (environmentConfig.OutputType ==  EnvironmentOutputType.Command && 
            !string.IsNullOrWhiteSpace(environmentConfig.OutputCommand))
        {
            await HandleFileWithCommand(fileInfo, environmentConfig, cancellationToken);
        }

        if (environmentConfig.OutputType == EnvironmentOutputType.Path &&
            !string.IsNullOrWhiteSpace(environmentConfig.OutputPath))
        {
            await HandleFileWithCopy(fileInfo, environmentConfig, cancellationToken);
        }
    }

    private async Task HandleFileWithCopy(IFileInfo fileInfo, EnvironmentConfig environmentConfig, CancellationToken cancellationToken)
    {
        logger.LogInformation("Copying file {PhysicalPath} to {OutputPath}",  fileInfo.PhysicalPath, environmentConfig.OutputPath);
        
        if (fileInfo.PhysicalPath == null)
        {
            logger.LogError("No file exists for {PhysicalPath}",  fileInfo.PhysicalPath);
            return;
        }

        Directory.CreateDirectory(environmentConfig.OutputPath);
        string fileName = fileInfo.Name;
        string destinationPath = Path.Combine(environmentConfig.OutputPath, fileName);

        FileStream source;
        await using (source = new FileStream(
                         fileInfo.PhysicalPath,
                         FileMode.Open,
                         FileAccess.Read,
                         FileShare.Read,
                         bufferSize: 81920,
                         useAsync: true))
        {

            await using var destination = new FileStream(
                destinationPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            await source.CopyToAsync(destination, cancellationToken);
        }

        source.Close();
        
        try
        {
            File.Delete(fileInfo.PhysicalPath);
        }
        catch (Exception)
        {
            logger.LogError("File copied but could not delete the source file: {PhysicalPath}",  fileInfo.PhysicalPath);
        }
    }

    private async Task HandleFileWithCommand(IFileInfo fileInfo, EnvironmentConfig environmentConfig, CancellationToken cancellationToken)
    {
        var command = BuildCommandString(fileInfo, environmentConfig);
        using var process = ExecuteCommand(command);
        var stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        await stdOutTask;
        var stdErr = await stdErrTask;

        if (process.ExitCode != 0)
        {
            throw new Exception($"OutputCommand failed with exit code {process.ExitCode}. stderr: {stdErr}");
        }
    }

    private Process ExecuteCommand(string command)
    {
        Process? process = null;
        try
        {
            ProcessStartInfo psi;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            else
            {
                psi = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }

            process = new Process { StartInfo = psi };
            process.Start();
            return process;
        }
        catch
        {
            process?.Dispose();
            throw;
        }
    }

    private string BuildCommandString(IFileInfo fileInfo, EnvironmentConfig environmentConfig)
    {
        logger.LogInformation("Building command from template: {Command}", environmentConfig.OutputCommand);
        var filePath = fileInfo.PhysicalPath ?? fileInfo.Name;
        var command = environmentConfig.OutputCommand.Replace("{file}", filePath, StringComparison.OrdinalIgnoreCase);
        
        if (!string.IsNullOrEmpty(environmentConfig.DestinationUsername))
        {
            command = command.Replace("{username}", environmentConfig.DestinationUsername, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrEmpty(environmentConfig.DestinationPassword))
        {
            command = command.Replace("{password}", environmentConfig.DestinationPassword, StringComparison.OrdinalIgnoreCase);
        }

        return command;
    }
}