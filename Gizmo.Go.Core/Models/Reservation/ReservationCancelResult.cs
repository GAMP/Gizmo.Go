namespace Gizmo.Go.Core.Models.Reservation
{
    /// <summary>
    /// Result of canceling a reservation.
    /// </summary>
    public enum ReservationCancelResult
    {
        /// <summary>
        /// The reservation is canceled.
        /// </summary>
        Success,

        /// <summary>
        /// The reservation can no longer be canceled (not found, not owned or already started).
        /// </summary>
        NotAvailable
    }
}
