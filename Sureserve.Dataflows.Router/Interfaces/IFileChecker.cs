using Sureserve.Dataflows.Router.Configuration;

namespace Sureserve.Dataflows.Router.Interfaces;

public interface IFileChecker
{
    Task CheckForFilesAsync(CancellationToken cancellationToken);
}