namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// A hall of a club with the layout of its hosts.
    /// </summary>
    public sealed class BookingHall
    {
        /// <summary>
        /// Gets the hall id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets the hall name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the display order.
        /// </summary>
        public int DisplayOrder { get; init; }

        /// <summary>
        /// Gets the hosts placed in the hall.
        /// </summary>
        public IReadOnlyList<BookingHallHost> Hosts { get; init; } = [];
    }
}
