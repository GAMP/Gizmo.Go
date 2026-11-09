using Gizmo.Go.Core.Models.Reservation;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Reservations of the current user.
    /// </summary>
    public interface IReservationService
    {
        /// <summary>
        /// Gets the current and upcoming reservations, start ascending.
        /// </summary>
        Task<IReadOnlyList<Reservation>> GetCurrentAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the latest completed, canceled and expired reservations, newest first.
        /// </summary>
        /// <param name="limit">Maximum number of reservations.</param>
        Task<IReadOnlyList<Reservation>> GetCompletedAsync(int limit, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels a not started reservation owned by the user.
        /// </summary>
        Task<ReservationCancelResult> CancelAsync(int id, CancellationToken cancellationToken = default);
    }
}
