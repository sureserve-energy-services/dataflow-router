using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.FileProviders;
using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.FileParsers;
using Sureserve.Dataflows.Router.Interfaces;

namespace Sureserve.Dataflows.Router.FileProcessing;

public class ElectricFileProcessor(IEnvironmentConfig environmentConfig) : IFileProcessor
{
    public async Task ProcessFileAsync(IFileInfo fileInfo, CancellationToken cancellationToken)
    {
        if (fileInfo != null)
        {
            if (fileInfo.PhysicalPath != null)
            {
                using StreamReader reader = File.OpenText(fileInfo.PhysicalPath);
                string flag = ElectricFileParser.GetFlagFromFileHeader(await reader.ReadToEndAsync());
                await HandleFileByFlag(flag, fileInfo, cancellationToken);
            }
        }
    }

    private async Task HandleFileByFlag(string flag, IFileInfo fileInfo, CancellationToken cancellationToken)
    {
        var commandTemplate = environmentConfig.OutputCommand;
        if (string.IsNullOrWhiteSpace(commandTemplate))
        {
            return;
        }

        var command = BuildCommandString(fileInfo, commandTemplate);
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

    private static Process ExecuteCommand(string command)
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

    private string BuildCommandString(IFileInfo fileInfo, string commandTemplate)
    {
        // Build the replacements
        var filePath = fileInfo.PhysicalPath ?? fileInfo.Name;

        // Basic required replacement
        var command = commandTemplate.Replace("{file}", filePath, StringComparison.OrdinalIgnoreCase);

        // Optional: if your command template also has {username} and {password}, you can replace them too
        // (Assumes these exist on EnvironmentConfig; adjust as needed)
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