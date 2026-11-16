namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// A host placed in a hall.
    /// </summary>
    public sealed class BookingHallHost
    {
        /// <summary>
        /// Gets the host id.
        /// </summary>
        public int HostId { get; init; }

        /// <summary>
        /// Gets the layout row.
        /// </summary>
        public int Row { get; init; }

        /// <summary>
        /// Gets the layout column.
        /// </summary>
        public int Column { get; init; }
    }
}
