namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// Price of a host group for the requested period.
    /// </summary>
    public sealed class BookingHostGroupPrice
    {
        /// <summary>
        /// Gets the host group id.
        /// </summary>
        public int HostGroupId { get; init; }

        /// <summary>
        /// Gets the session price of one host; null when the host group cannot be booked for the period.
        /// </summary>
        public decimal? Price { get; init; }

        /// <summary>
        /// Gets the reference price of one hour (display only); null when the server has no tariff for it.
        /// </summary>
        public decimal? HourlyRate { get; init; }
    }
}
