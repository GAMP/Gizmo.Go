namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// Result of creating an offer.
    /// </summary>
    public sealed class BookingOfferResult
    {
        /// <summary>
        /// Gets the result code.
        /// </summary>
        public BookingOfferResultCode Code { get; init; }

        /// <summary>
        /// Gets the offer; set on <see cref="BookingOfferResultCode.Success"/>.
        /// </summary>
        public BookingOffer? Offer { get; init; }

        /// <summary>
        /// Gets the ids of the hosts that cannot be booked.
        /// </summary>
        public IReadOnlyList<int> UnavailableHostIds { get; init; } = [];
    }
}
