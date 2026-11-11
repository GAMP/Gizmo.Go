using Gizmo.Go.Core.Models.Events;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Real-time events of the current user (own balance and reservations, published news).
    /// </summary>
    public interface IUserEventsService
    {
        /// <summary>
        /// Occurs when an event of the user is received. May be raised on any thread.
        /// </summary>
        event EventHandler<UserEvent>? EventReceived;

        /// <summary>
        /// Starts the connection and keeps it alive (reconnects) until <see cref="StopAsync"/>. Does nothing when already started.
        /// A server that cannot be reached is retried in the background and is not an error.
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the connection.
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
