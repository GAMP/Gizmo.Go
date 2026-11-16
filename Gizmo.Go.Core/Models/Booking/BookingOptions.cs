namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// Reservation options of the server the booking flow depends on.
    /// </summary>
    public sealed class BookingOptions
    {
        /// <summary>
        /// Gets the minimum session duration in minutes; null when not limited.
        /// </summary>
        public int? MinimumTime { get; init; }

        /// <summary>
        /// Gets the maximum session duration in minutes; null when not limited.
        /// </summary>
        public int? MaximumTime { get; init; }

        /// <summary>
        /// Gets a value indicating whether one reservation may hold only one host.
        /// </summary>
        public bool MultiHostDisable { get; init; }

        /// <summary>
        /// Gets the reservation fee type.
        /// </summary>
        public BookingFeeType FeeType { get; init; }

        /// <summary>
        /// Gets the reservation fee value (an amount or a percentage by <see cref="FeeType"/>).
        /// </summary>
        public decimal Fee { get; init; }

        /// <summary>
        /// Gets the minimum payment percentage of the total.
        /// </summary>
        public decimal MinimumPaymentPercentage { get; init; }

        /// <summary>
        /// Gets the cancellation grace period in minutes.
        /// </summary>
        public int? CancellationGracePeriod { get; init; }
    }
}
