namespace Gizmo.Go.Core.Models.Profile
{
    /// <summary>
    /// Result of changing the password of the current user.
    /// </summary>
    public enum UserPasswordChangeCode
    {
        Success,

        /// <summary>
        /// The current password is wrong.
        /// </summary>
        WrongPassword,

        /// <summary>
        /// The new password does not satisfy the password rules of the server.
        /// </summary>
        InvalidPassword
    }
}
