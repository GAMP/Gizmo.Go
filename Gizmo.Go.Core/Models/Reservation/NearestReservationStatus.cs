namespace Gizmo.Go.Core.Models.Reservation
{
    /// <summary>
    /// Status of the nearest reservation.
    /// </summary>
    public enum NearestReservationStatus
    {
        /// <summary>
        /// Confirmed, not started.
        /// </summary>
        Waiting,

        /// <summary>
        /// In progress.
        /// </summary>
        Active
    }
}
