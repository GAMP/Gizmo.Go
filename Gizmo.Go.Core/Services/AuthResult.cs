namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Authentication result.
    /// </summary>
    public sealed class AuthResult
    {
        /// <summary>
        /// Gets whether authentication succeeded.
        /// </summary>
        public required bool Success { get; init; }

        /// <summary>
        /// Gets error code on failure.
        /// </summary>
        public AuthErrorCode? ErrorCode { get; init; }

        public static AuthResult Succeeded() => new() { Success = true };

        public static AuthResult Failed(AuthErrorCode errorCode) => new() { Success = false, ErrorCode = errorCode };
    }
}
