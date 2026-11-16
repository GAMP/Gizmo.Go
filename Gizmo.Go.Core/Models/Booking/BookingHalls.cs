namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// Halls of a club with the bookable hosts and their host groups.
    /// </summary>
    public sealed class BookingHalls
    {
        /// <summary>
        /// Gets the halls ordered by display order; empty when the club has no halls.
        /// </summary>
        public IReadOnlyList<BookingHall> Halls { get; init; } = [];

        /// <summary>
        /// Gets the bookable hosts of the club.
        /// </summary>
        public IReadOnlyList<BookingHost> Hosts { get; init; } = [];

        /// <summary>
        /// Gets the host groups of the bookable hosts.
        /// </summary>
        public IReadOnlyList<BookingHostGroup> HostGroups { get; init; } = [];
    }
}
