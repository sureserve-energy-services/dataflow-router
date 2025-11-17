namespace Sureserve.Dataflows.Router.Configuration;

public interface IEnvironmentConfig
{
    string InputPath { get; set; }
    List<string> FileExtensions { get; set; }
    string Flag { get; set; }
    EnvironmentOutputType OutputType { get; set; }
    string OutputCommand { get; set; }
    string OutputPath { get; set; }
    EnvironmentType EnvironmentType { get; set; }
}