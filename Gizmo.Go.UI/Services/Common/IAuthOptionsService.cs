namespace Gizmo.Go.UI.Services.Common
{
    /// <summary>
    /// Server options of the auth screens, loaded on demand by the screens that need them (never on the app start).
    /// </summary>
    public interface IAuthOptionsService
    {
        /// <summary>
        /// Gets the options. A successful result is cached for the app run; a failure is logged and retried on the next call.
        /// </summary>
        /// <returns>The options, or null when the server options could not be loaded.</returns>
        Task<AuthOptions?> GetAsync(CancellationToken cancellationToken = default);
    }
}
