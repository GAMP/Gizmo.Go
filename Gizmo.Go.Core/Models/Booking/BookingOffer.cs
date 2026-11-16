namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// A reservation offer: the cost of the chosen hosts for the period.
    /// </summary>
    public sealed class BookingOffer
    {
        /// <summary>
        /// Gets the offer id.
        /// </summary>
        public Guid OfferId { get; init; }

        /// <summary>
        /// Gets the session start (UTC).
        /// </summary>
        public DateTime Date { get; init; }

        /// <summary>
        /// Gets the session duration in minutes.
        /// </summary>
        public int Duration { get; init; }

        /// <summary>
        /// Gets the hosts of the offer.
        /// </summary>
        public IReadOnlyList<BookingOfferHost> Hosts { get; init; } = [];

        /// <summary>
        /// Gets the reservation fee; null when there is none.
        /// </summary>
        public decimal? Fee { get; init; }

        /// <summary>
        /// Gets the discount.
        /// </summary>
        public decimal Discount { get; init; }

        /// <summary>
        /// Gets the total to pay.
        /// </summary>
        public decimal Total { get; init; }

        /// <summary>
        /// Gets the minimum payment that confirms the reservation.
        /// </summary>
        public decimal MinimumPayment { get; init; }
    }
}
