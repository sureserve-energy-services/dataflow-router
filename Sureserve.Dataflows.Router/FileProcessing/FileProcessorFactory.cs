using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.Interfaces;

namespace Sureserve.Dataflows.Router.FileProcessing;

public static class FileProcessorFactory
{
    public static IFileProcessor Create(string fileExtension, IEnvironmentConfig environmentConfig)
    {
        if (environmentConfig.FileExtensions.Select(fe => fe.ToLower()).Contains(fileExtension.ToLower()))
        {
            if (environmentConfig.EnvironmentType == EnvironmentType.Electric)
            {
                return new ElectricFileProcessor();
            }
            
            if (environmentConfig.EnvironmentType == EnvironmentType.Gas)
            {
                return new GasFileProcessor();
            }
        }

        throw new NotSupportedException($"File extension '{fileExtension}' is not supported.");
    }
}