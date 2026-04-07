namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Opens an external URL outside the application — in the system browser or target app.
    /// </summary>
    public interface IExternalLauncher
    {
        Task OpenAsync(string url, CancellationToken cancellationToken = default);
    }
}
