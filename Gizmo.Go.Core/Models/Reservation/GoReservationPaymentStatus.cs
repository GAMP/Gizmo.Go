namespace Gizmo.Go.Core.Models.Reservation
{
    /// <summary>
    /// Payment status of a reservation, with the values of the server.
    /// </summary>
    public enum GoReservationPaymentStatus
    {
        /// <summary>
        /// Payment is not required.
        /// </summary>
        NotRequired = 0,

        /// <summary>
        /// Not paid.
        /// </summary>
        NotSatisfied = 1,

        /// <summary>
        /// Partially paid.
        /// </summary>
        PartiallySatisfied = 2,

        /// <summary>
        /// Paid.
        /// </summary>
        Satisfied = 3
    }
}
