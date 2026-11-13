namespace Gizmo.Go.Core.Models.Reservation
{
    /// <summary>
    /// Status of a reservation, with the values of the server.
    /// </summary>
    public enum GoReservationStatus
    {
        /// <summary>
        /// Not started yet.
        /// </summary>
        Waiting = 0,

        /// <summary>
        /// Canceled.
        /// </summary>
        Canceled = 1,

        /// <summary>
        /// In progress.
        /// </summary>
        Active = 2,

        /// <summary>
        /// Expired without being activated.
        /// </summary>
        Expired = 3,

        /// <summary>
        /// Completed.
        /// </summary>
        Completed = 4,

        /// <summary>
        /// Released.
        /// </summary>
        Released = 5
    }
}
