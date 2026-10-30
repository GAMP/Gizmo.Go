namespace Gizmo.Go.UI.Services.Common
{
    /// <summary>
    /// Post-login steps, after the client UserLoginStatusViewService: pending user agreements, then profile completion,
    /// then the landing page. Runs after login and on every app start with a restored session.
    /// </summary>
    public interface IUserLoginStatusService
    {
        /// <summary>
        /// Runs the post-login chain.
        /// </summary>
        /// <param name="returnUrl">Where to go at the end; empty keeps the current route (Welcome and Login go to home).</param>
        Task RunAsync(string? returnUrl, CancellationToken cancellationToken = default);
    }
}
