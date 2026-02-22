namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Authentication error code.
    /// </summary>
    public enum AuthErrorCode
    {
        /// <summary>
        /// Invalid credentials (wrong username or password).
        /// </summary>
        InvalidCredentials,

        /// <summary>
        /// Connection to the server failed.
        /// </summary>
        ConnectionFailed,

        /// <summary>
        /// An unexpected error occurred.
        /// </summary>
        Unexpected,
    }
}
