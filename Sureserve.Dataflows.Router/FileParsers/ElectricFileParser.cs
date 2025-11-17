namespace Sureserve.Dataflows.Router.FileParsers;

public static class ElectricFileParser
{
    public static string GetFlagFromFileHeader(string fileContents)
    {
        string[] rows = fileContents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        string[] headerColumns = rows[0].Split('|');
        return headerColumns[10];
    }
}