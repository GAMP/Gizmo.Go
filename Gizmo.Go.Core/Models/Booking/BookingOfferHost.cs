namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// A host of an offer with its price.
    /// </summary>
    public sealed class BookingOfferHost
    {
        /// <summary>
        /// Gets the host id.
        /// </summary>
        public int HostId { get; init; }

        /// <summary>
        /// Gets the host group id.
        /// </summary>
        public int HostGroupId { get; init; }

        /// <summary>
        /// Gets the price of the host for the session.
        /// </summary>
        public decimal Price { get; init; }
    }
}
