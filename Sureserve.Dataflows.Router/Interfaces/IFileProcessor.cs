namespace Sureserve.Dataflows.Router.Interfaces;

public interface IFileProcessor
{
    Task ProcessFilesAsync(CancellationToken cancellationToken);
}