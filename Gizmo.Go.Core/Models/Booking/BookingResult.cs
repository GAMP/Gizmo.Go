namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// Result of booking an offer.
    /// </summary>
    public sealed class BookingResult
    {
        /// <summary>
        /// Gets the result code.
        /// </summary>
        public BookingResultCode Code { get; init; }

        /// <summary>
        /// Gets the created reservation id; set on <see cref="BookingResultCode.Success"/>.
        /// </summary>
        public int? ReservationId { get; init; }

        /// <summary>
        /// Gets the ids of the hosts that are no longer free.
        /// </summary>
        public IReadOnlyList<int> UnavailableHostIds { get; init; } = [];
    }
}
