using Microsoft.Extensions.FileProviders;
using Sureserve.Dataflows.Router.Interfaces;

namespace Sureserve.Dataflows.Router.FileProcessing;

public class GasFileProcessor : IFileProcessor
{
    public Task ProcessFileAsync(IFileInfo fileInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}