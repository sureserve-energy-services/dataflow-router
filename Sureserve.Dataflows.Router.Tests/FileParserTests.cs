using System.Text;
using Sureserve.Dataflows.Router.FileParsers;

namespace Sureserve.Dataflows.Router.Tests;

public class FileParserTests
{
    [Fact]
    public void ElectricFileParser_Returns_Correct_Flag()
    {
        // Arrange
        string fileContents = GetFileWithProductionFlag();

        // Act
        string flagResult = ElectricFileParser.GetFlagFromFileHeader(fileContents);

        // Assert
        Assert.Equal("OPER", flagResult);
    }
    
    private string GetFileWithProductionFlag()
    {
        StringBuilder dataflow = new();
        dataflow.Append($"ZHV|DTS0018862|D0155001|X|SPOW|M|BMSL|20211217073407||||OPER|{Environment.NewLine}");
        dataflow.Append($"315|1300001498130|20211201|||2F 4||WATCHMAN BRAE|||ABERDEEN||AB21 9WF|BMSLSPOWMO|N|_P|{Environment.NewLine}");
        dataflow.Append($"317|20211201|{Environment.NewLine}");
        dataflow.Append($"318|0001|0001|{Environment.NewLine}");
        dataflow.Append($"315|1700053330391|20211217|||11||GREENFIELD CRESCENT|DUNDEE||DUNDEE||DD4 0FT|BMSLSPOWMO|N|_P|{Environment.NewLine}");
        dataflow.Append($"317|20211217|{Environment.NewLine}");
        dataflow.Append($"318|0001|0001|{Environment.NewLine}");
        dataflow.Append($"ZPT|DTS0018862|144||48|20211217073408|");
        return dataflow.ToString();
    }
}
