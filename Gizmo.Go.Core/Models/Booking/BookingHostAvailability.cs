namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// Whether a host is free for the requested period.
    /// </summary>
    public sealed class BookingHostAvailability
    {
        /// <summary>
        /// Gets the host id.
        /// </summary>
        public int HostId { get; init; }

        /// <summary>
        /// Gets a value indicating whether the host is free for the user for the period.
        /// </summary>
        public bool IsAvailable { get; init; }
    }
}
