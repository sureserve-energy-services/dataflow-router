using Sureserve.Dataflows.Router.Configuration;
using Sureserve.Dataflows.Router.FileProcessing;
using Sureserve.Dataflows.Router.Interfaces;

namespace Sureserve.Dataflows.Router.Tests;

public class FileProcessorFactoryTests
{
    [Fact]
    public void FileProcessorFactory_Returns_Correct_Processor_Electric_LowerCase()
    {
        // Arrange & Act
        IFileProcessor processor = FileProcessorFactory.Create("usr", GetElectricConfig());
        
        // Assert
        Assert.IsType<ElectricFileProcessor>(processor);
    }
    
    [Fact]
    public void FileProcessorFactory_Returns_Correct_Processor_Electric_UpperCase()
    {
        // Arrange & Act
        IEnvironmentConfig config = GetElectricConfig();
        config.FileExtensions = ["USR"];
        IFileProcessor processor = FileProcessorFactory.Create("usr", config);
        
        // Assert
        Assert.IsType<ElectricFileProcessor>(processor);
    }

    private IEnvironmentConfig GetElectricConfig()
    {
        return new EnvironmentConfig()
        {
            EnvironmentType = EnvironmentType.Electric,
            FileExtensions = ["usr"],
            Flag = "OPER",
            InputPath = "",
            OutputCommand = "",
        };
    }
}