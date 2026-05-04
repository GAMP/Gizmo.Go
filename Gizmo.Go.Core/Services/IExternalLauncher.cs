namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Opens an external URL outside the application — in the system browser or target app.
    /// </summary>
    public interface IExternalLauncher
    {
        Task OpenAsync(string url, CancellationToken cancellationToken = default);

        ValueTask<IDisposable?> OpenPlaceholderAsync(CancellationToken ct = default);
        ValueTask RedirectPlaceholderAsync(IDisposable? placeholder, string url, CancellationToken ct = default);
        ValueTask ClosePlaceholderAsync(IDisposable? placeholder, CancellationToken ct = default);
    }
}
