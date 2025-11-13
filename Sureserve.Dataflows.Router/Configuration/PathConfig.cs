namespace Sureserve.Dataflows.Router.Configuration;

public class PathConfig
{
    public string GasDataFlowsIncomingFolderPath { get; set; } = "";
    public string ElectricDataFlowsIncomingFolderPath { get; set; } = "";
    public string GasDataFlowsOutgoingFolderPath { get; set; } = "";
    public string ElectricDataFlowsOutgoingFolderPath { get; set; } = "";
}