namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// A host group (zone) of the bookable hosts.
    /// </summary>
    public sealed class BookingHostGroup
    {
        /// <summary>
        /// Gets the host group id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets the host group name.
        /// </summary>
        public string Name { get; init; } = string.Empty;
    }
}
