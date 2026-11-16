namespace Gizmo.Go.Core.Models.Booking
{
    /// <summary>
    /// Result code of creating an offer.
    /// </summary>
    public enum BookingOfferResultCode
    {
        /// <summary>
        /// The offer is created.
        /// </summary>
        Success,

        /// <summary>
        /// Some of the hosts are no longer free.
        /// </summary>
        HostsUnavailable,

        /// <summary>
        /// A host group of the chosen hosts has no price for the period.
        /// </summary>
        NoPrice
    }
}
