namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Authentication state.
    /// </summary>
    public enum AuthState
    {
        /// <summary>
        /// State not yet determined.
        /// </summary>
        Unknown,

        /// <summary>
        /// Authentication in progress.
        /// </summary>
        Authenticating,

        /// <summary>
        /// User is authenticated.
        /// </summary>
        Authenticated,

        /// <summary>
        /// User is not authenticated.
        /// </summary>
        Unauthenticated
    }
}
