using Microsoft.Extensions.FileProviders;

namespace Sureserve.Dataflows.Router.Interfaces;

public interface IFileProcessor
{
    Task ProcessFileAsync(IFileInfo fileInfo, CancellationToken cancellationToken);
}