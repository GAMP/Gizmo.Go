namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// A bookable host of a club.
    /// </summary>
    public sealed class BookingHost
    {
        /// <summary>
        /// Gets the host id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets the host number.
        /// </summary>
        public int Number { get; init; }

        /// <summary>
        /// Gets the host name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the host group id.
        /// </summary>
        public int HostGroupId { get; init; }
    }
}
