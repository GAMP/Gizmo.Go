namespace Gizmo.Go.UI.Services.Common
{
    /// <summary>
    /// In-app toasts for the real-time events of the logged in user (own reservations and balance, published news),
    /// filtered by the notification switches of the device.
    /// </summary>
    public interface IUserNotificationsService
    {
        /// <summary>
        /// Starts following the authentication state and the app lifecycle; connects at once when the user is already authenticated.
        /// Called once after the app initialization.
        /// </summary>
        void Run();
    }
}
