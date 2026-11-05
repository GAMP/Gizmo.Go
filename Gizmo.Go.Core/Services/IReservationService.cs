using Gizmo.Go.Core.Models.Reservation;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Reservations of the current user.
    /// </summary>
    public interface IReservationService
    {
        /// <summary>
        /// Gets the nearest reservation; null when there is none.
        /// </summary>
        Task<NearestReservation?> GetNearestAsync(CancellationToken cancellationToken = default);
    }
}
