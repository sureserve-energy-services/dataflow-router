namespace Sureserve.Dataflows.Router.Configuration;

public class EnvironmentConfig : IEnvironmentConfig
{
    public EnvironmentType EnvironmentType { get; set; }
    public List<string> FileExtensions { get; set; } = [];
    public string InputPath { get; set; } = "";
    public string Flag { get; set; } = "";
    public EnvironmentOutputType OutputType { get; set; } = EnvironmentOutputType.Path;
    public string OutputCommand { get; set; } = "";
    public string OutputPath { get; set; } = "";
}